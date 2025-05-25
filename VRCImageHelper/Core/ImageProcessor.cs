namespace VRCImageHelper.Core;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using VRCImageHelper.Tools;

#pragma warning disable IDE1006 
public struct Placeholders
{
    public const string TakenYear = "%{TAKEN:yyyy}%";
    public const string TakenMonth = "%{TAKEN:MM}%";
    public const string TakenDay = "%{TAKEN:dd}%";
    public const string TakenHour = "%{TAKEN:hh}%";
    public const string TakenMinute = "%{TAKEN:mm}%";
    public const string TakenSecond = "%{TAKEN:ss}%";
    public const string TakenMillisecond = "%{TAKEN:fff}%";

    public const string JoinYear = "%{JOIN:yyyy}%";
    public const string JoinMonth = "%{JOIN:MM}%";
    public const string JoinDay = "%{JOIN:dd}%";
    public const string JoinHour = "%{JOIN:hh}%";
    public const string JoinMinute = "%{JOIN:mm}%";
    public const string JoinSecond = "%{JOIN:ss}%";

    public const string Year = "yyyy";
    public const string Month = "MM";
    public const string Day = "dd";
    public const string Hour = "hh";
    public const string Minute = "mm";
    public const string Second = "ss";
    public const string Millisecond = "fff";
    public const string Width = "XXXX";
    public const string Height = "YYYY";

    public const string World = "%{WORLD}%";
    public const string WorldId = "%{WORLD:ID}%";
    public const string InstanceId = "%{INSTANCE:ID}%";
    public const string InstanceType = "%{INSTANCE:TYPE}%";
    public const string OwnerId = "%{OWNER:ID}%";

    public const string MultiLayerPostfix = "%{_LAYER}%";

    public const string Camera = "%{CAMERA}%";

    public static List<string> Keys()
    {
        return typeof(Placeholders)
            .GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
            .Select(f => f.GetValue(null))
            .Where(value => value is string)
            .Select(value => (string)value!)
            .ToList();
    }
}
#pragma warning restore IDE1006

/// <summary>
/// 画像処理を行うクラス
/// </summary>
internal class ImageProcessor
{
    private struct ImageInfo
    {
        public bool HasAlpha { get; set; }
        public bool IsPrint { get; set; }
    }

    /// <summary>
    /// 画像を処理する
    /// </summary>
    public static void ProcessImage(string sourcePath, State state)
    {
        if (!new FileInfo(sourcePath).Exists)
            throw new FileNotFoundException($"ソースファイルが見つかりません: {sourcePath}");

        var imageInfo = AnalyzeImage(sourcePath);
        var filePath = FormatFilePath(Path.GetFileName(sourcePath), state, imageInfo.HasAlpha, imageInfo.IsPrint);
        var destPath = BuildDestinationPath(sourcePath, filePath);

        if (destPath == null)
            throw new InvalidOperationException("出力先パスの生成に失敗しました");

        if (!ValidateDestination(destPath))
            throw new InvalidOperationException($"出力先の検証に失敗しました: {destPath}");

        var tmpPath = CompressImage(sourcePath, imageInfo.HasAlpha);
        if (!new FileInfo(tmpPath).Exists)
            throw new InvalidOperationException($"画像圧縮に失敗しました: {sourcePath}");

        if (WriteMetadata(tmpPath, destPath, state) != true)
            throw new InvalidOperationException($"メタデータの書き込みに失敗しました: {tmpPath}");

        if (ConfigManager.DeleteOriginalFile)
        {
            DeleteOriginalFile(sourcePath);
        }

        UI.SendNotify.Send("OK!", false);
    }

    #region Image Analysis

    private static ImageInfo AnalyzeImage(string sourcePath)
    {
        using var targetImage = new Bitmap(sourcePath);

        var isPrint = IsPrintImage(targetImage);
        var hasAlpha = !isPrint && HasAlphaChannel(targetImage);

        return new ImageInfo
        {
            HasAlpha = hasAlpha,
            IsPrint = isPrint
        };
    }

    private static bool IsPrintImage(Bitmap image)
    {
        if (image.Width != 2048 || image.Height != 1440) return false;

        return CheckCornersAreWhite(image);
    }

    private static bool CheckCornersAreWhite(Bitmap image)
    {
        static bool IsPureWhite(Color color)
        {
            return color.R == 255 && color.G == 255 && color.B == 255;
        }

        var topLeft = image.GetPixel(0, 0);
        var topRight = image.GetPixel(image.Width - 1, 0);
        var bottomLeft = image.GetPixel(0, image.Height - 1);
        var bottomRight = image.GetPixel(image.Width - 1, image.Height - 1);

        return IsPureWhite(topLeft)
            && IsPureWhite(topRight)
            && IsPureWhite(bottomLeft)
            && IsPureWhite(bottomRight);
    }

    private static bool HasAlphaChannel(Bitmap image)
    {
        var formatWithAlpha = new PixelFormat[]
        {
            PixelFormat.Alpha,
            PixelFormat.Canonical,
            PixelFormat.Format16bppArgb1555,
            PixelFormat.Format32bppArgb,
            PixelFormat.Format32bppPArgb,
            PixelFormat.Format64bppArgb,
            PixelFormat.Format64bppPArgb
        };

        return formatWithAlpha.Contains(image.PixelFormat);
    }

    #endregion

    #region Path Management

    private static string? BuildDestinationPath(string sourcePath, string filePath)
    {
        filePath = Regex.Replace(filePath, @"[<>:""|?*]", "_");

        var destPath = GetDestinationDirectory(sourcePath);
        if (destPath == null) return null;

        var basePath = Path.GetFullPath(destPath);
        destPath = Path.Combine(destPath, filePath);

        // パストラバーサル攻撃の防止
        if (!Path.GetFullPath(destPath).StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return destPath;
    }

    private static string? GetDestinationDirectory(string sourcePath)
    {
        var destPath = ConfigManager.DestDir;
        if (destPath == "")
        {
            var sourceDir = Path.GetDirectoryName(sourcePath);
            if (sourceDir is null) return null;

            destPath = new DirectoryInfo(sourceDir)?.Parent?.FullName;
            if (destPath is null) return null;
        }

        return destPath;
    }

    private static bool ValidateDestination(string destPath)
    {
        var destDir = Path.GetDirectoryName(destPath);
        if (destDir is null) return false;

        if (Directory.CreateDirectory(destDir).Exists == false)
        {
            UI.SendNotify.Send(Properties.Resources.NotifyErrorImageProcessCantCreateDirectory, false);
            return false;
        }

        if (!ConfigManager.OverwriteDestinationFile && new FileInfo(destPath).Exists)
        {
            UI.SendNotify.Send(Properties.Resources.NotifyErrorImageProcessFileExist, false);
            return false;
        }

        return true;
    }

    private static void DeleteOriginalFile(string sourcePath)
    {
        try
        {
            File.Delete(sourcePath);
        }
        catch (IOException ex)
        {
            UI.SendNotify.Send(Properties.Resources.NotifyErrorImageProcessCantDeleteOriginal + ":\n" + ex.Message, false);
        }
    }

    #endregion

    #region File Naming

    private static string FormatFilePath(string sourceFile, State state, bool hasAlpha, bool isPrint)
    {
        var placeholders = BuildPlaceholders(sourceFile, state, isPrint);
        var filePath = hasAlpha ? ConfigManager.AlphaFilePattern : ConfigManager.FilePattern;

        foreach (var entry in placeholders)
        {
            filePath = filePath.Replace(entry.Key, entry.Value);
        }

        return filePath;
    }

    private static Dictionary<string, string> BuildPlaceholders(string sourceFile, State state, bool isPrint)
    {
        var joinMatch = Regex.Match(state.RoomInfo.JoinDateTime, @"(?<Year>\d+)\.(?<Month>\d+)\.(?<Day>\d+) (?<Hour>\d+):(?<Minute>\d+):(?<Second>\d+)");
        var takenMatch = Regex.Match(sourceFile, @"(?<Year>\d+)-(?<Month>\d+)-(?<Day>\d+)_(?<Hour>\d+)-(?<Minute>\d+)-(?<Second>\d+)\.(?<Millisecond>\d+)_(?<Width>\d+)x(?<Height>\d+)(?<MultiLayer>_[A-Za-z]+)?");

        var instanceType = GetInstanceType(state.RoomInfo.Permission);
        var cameraType = GetCameraType(isPrint, state);

        return new Dictionary<string, string>
        {
            { Placeholders.TakenYear, takenMatch.Groups["Year"].Value },
            { Placeholders.TakenMonth, takenMatch.Groups["Month"].Value },
            { Placeholders.TakenDay, takenMatch.Groups["Day"].Value },
            { Placeholders.TakenHour, takenMatch.Groups["Hour"].Value },
            { Placeholders.TakenMinute, takenMatch.Groups["Minute"].Value },
            { Placeholders.TakenSecond, takenMatch.Groups["Second"].Value },
            { Placeholders.TakenMillisecond, takenMatch.Groups["Millisecond"].Value },

            { Placeholders.JoinYear, joinMatch.Groups["Year"].Value },
            { Placeholders.JoinMonth, joinMatch.Groups["Month"].Value },
            { Placeholders.JoinDay, joinMatch.Groups["Day"].Value },
            { Placeholders.JoinHour, joinMatch.Groups["Hour"].Value },
            { Placeholders.JoinMinute, joinMatch.Groups["Minute"].Value },
            { Placeholders.JoinSecond, joinMatch.Groups["Second"].Value },

            { Placeholders.Year, takenMatch.Groups["Year"].Value },
            { Placeholders.Month, takenMatch.Groups["Month"].Value },
            { Placeholders.Day, takenMatch.Groups["Day"].Value },
            { Placeholders.Hour, takenMatch.Groups["Hour"].Value },
            { Placeholders.Minute, takenMatch.Groups["Minute"].Value },
            { Placeholders.Second, takenMatch.Groups["Second"].Value },
            { Placeholders.Millisecond, takenMatch.Groups["Millisecond"].Value },
            { Placeholders.Width, takenMatch.Groups["Width"].Value },
            { Placeholders.Height, takenMatch.Groups["Height"].Value },

            { Placeholders.MultiLayerPostfix, takenMatch.Groups["MultiLayer"].Value },

            { Placeholders.World, string.IsNullOrEmpty(state.RoomInfo.World_name) ? "UNKNOWN WORLD" : state.RoomInfo.World_name },
            { Placeholders.WorldId, state.RoomInfo.World_id },
            { Placeholders.InstanceId, state.RoomInfo.Instance_id},
            { Placeholders.InstanceType, instanceType },
            { Placeholders.OwnerId, state.RoomInfo.Organizer },

            { Placeholders.Camera, cameraType }
        };
    }

    private static string GetInstanceType(string permission)
    {
        return permission switch
        {
            "hidden" => "Friends+",
            "friends" => "Friends",
            "private" => "Invite",
            "private_plus" => "Invite+",
            "group_public" => "GroupPublic",
            "group_members" => "Group",
            "group_plus" => "Group+",
            _ => "Public",
        };
    }

    private static string GetCameraType(bool isPrint, State state)
    {
        return isPrint ? "Print"
             : state.VirtualLens2.Enabled ? "VirtualLens2"
             : state.Integral.Enabled ? "Integral"
             : "VRCCamera";
    }

    #endregion

    #region Image Compression

    private static string CompressImage(string sourcePath, bool hasAlpha)
    {
        var destPath = Path.GetTempFileName();
        File.Delete(destPath);
        var format = hasAlpha ? ConfigManager.AlphaFormat : ConfigManager.Format;
        var quality = hasAlpha ? ConfigManager.AlphaQuality : ConfigManager.Quality;

        switch (format)
        {
            case "AVIF":
                CompressAVIF(sourcePath, destPath, quality, hasAlpha);
                break;

            case "WEBP":
                CompressWEBP(sourcePath, destPath, quality, hasAlpha);
                break;

            case "JPEG":
                CompressJPEG(sourcePath, destPath, 100 - quality);
                break;

            case "PNG":
                File.Copy(sourcePath, destPath);
                break;
        }

        return destPath;
    }

    private static void CompressJPEG(string src, string dest, int quality)
    {
        using var image = new Bitmap(src);
        var encoder = ImageCodecInfo.GetImageEncoders().ToList()
                        .Where(e => e.FormatID == ImageFormat.Jpeg.Guid)
                        .First();

        var encodeParams = new EncoderParameters(1);
        encodeParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

        image.Save(dest, encoder, encodeParams);
    }

    private static void CompressAVIF(string src, string dest, int quality, bool hasAlpha)
    {
        var encoder = hasAlpha ? ConfigManager.AlphaEncoder : ConfigManager.Encoder;
        var option = hasAlpha ? ConfigManager.AlphaEncoderOption : ConfigManager.EncoderOption;

        FFMpeg.Encode(src, dest, "avif", encoder, quality, option).Wait();
    }

    private static void CompressWEBP(string src, string dest, int quality, bool hasAlpha)
    {
        var encoder = hasAlpha ? ConfigManager.AlphaEncoder : ConfigManager.Encoder;
        var option = hasAlpha ? ConfigManager.AlphaEncoderOption : ConfigManager.EncoderOption;

        FFMpeg.Encode(src, dest, "webp", encoder, quality, option).Wait();
    }

    #endregion

    #region Metadata

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="destPath"></param>
    /// <param name="state"></param>
    /// <returns>ExiftoolのExit Codeが0ならTrue それ以外ならFalse</returns>
    private static bool WriteMetadata(string path, string destPath, State state)
    {
        var desc = $"Taken at {state.RoomInfo.World_name}, with {string.Join(",", state.Players)}.";

        var makernote = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(state, options: new JsonSerializerOptions(JsonSerializerDefaults.Web) { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping })
            )
        );

        var args = BuildMetadataArgs(desc, makernote, state);

        var exifTool = ExifTool.Write(path, args);
        if (exifTool is not null)
        {
            exifTool.Wait();

            if (exifTool.Result)
            {
                try
                {
                    File.Move(path, destPath, ConfigManager.OverwriteDestinationFile);
                }
                catch (FileNotFoundException ex)
                {
                    UI.SendNotify.Send(Properties.Resources.NotifyErrorImageProcessExiftoolResultNotFound + ":\n" + ex.Message, false);
                }
                catch (IOException ex)
                {
                    UI.SendNotify.Send(Properties.Resources.NotifyErrorImageProcessFileExist + ":\n" + ex.Message, false);
                }
            }

            return exifTool.Result;
        }
        else
        {
            return false;
        }
    }

    private static List<string> BuildMetadataArgs(string desc, string makernote, State state)
    {
        var args = new List<string>
        {
            "-n",
            "-overwrite_original",
            "-codedcharacterset=utf8",
            $"-:ImageDescription={desc}",
            $"-:Description={desc}",
            $"-:Comment={desc}",
            $"-makernote={makernote}",
            "-sep \";\"",
            $"-:Keywords={state.RoomInfo.World_name};{string.Join(';', state.Players)}"
        };

        AddDateTimeArgs(args, state);
        AddCameraArgs(args, state);

        return args;
    }

    private static void AddDateTimeArgs(List<string> args, State state)
    {
        var offset = "";
        if (DateTime.TryParseExact(state.CreationDate, "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out var dT))
        {
            var offsetSpan = TimeZoneInfo.Local.GetUtcOffset(dT);
            var sign = offsetSpan > TimeSpan.Zero ? "+" : "";
            offset = sign + offsetSpan.ToString();
            args.Add($"-:OffsetTime={offset}");
            args.Add($"-:DateCreated={dT:yyyy:MM:dd}:");
            args.Add($"-:TimeCreated={dT:HH:mm:ss}{offset}");
        }

        args.Add($"-:CreateDate={state.CreationDate}{offset}");
        args.Add($"-:DateTimeOriginal={state.CreationDate}{offset}");
        args.Add($"-xmp-Photoshop:DateCreated={state.CreationDate}{offset}");
    }

    private static void AddCameraArgs(List<string> args, State state)
    {
        args.AddRange(StateChecker.VirtualLens2.Publish(state.VirtualLens2));
        args.AddRange(StateChecker.Integral.Publish(state.Integral));

        if (!(state.VirtualLens2.Enabled || state.Integral.Enabled))
        {
            args.Add("-:Make=VRChat");
            args.Add("-:Model=VRChat Camera");
        }
    }

    #endregion
}
