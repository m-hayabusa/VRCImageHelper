namespace VRCImageHelper.Core;

using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using VRCImageHelper.Tools;

internal class ImageProcess
{
    public static void Taken(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, @"([0-9.: ]*) (?:Log|Debug) +? -  \[VRC Camera\] Took screenshot to: (.*)");
        if (match.Success)
        {
            var state = State.Current.Clone();

            var creationDate = match.Groups[1].ToString().Replace('.', ':');
            state.CreationDate = creationDate;

            var path = match.Groups[2].ToString();

            new Task(() => Process(path, state)).Start();
        }
    }

    public static CancellationToken s_cancellationToken;
    public static SemaphoreSlimWrapper? s_compressSemaphore;

    static ImageProcess()
    {
        if (ConfigManager.ParallelCompressionProcesses > 0)
        {
            s_compressSemaphore = new SemaphoreSlimWrapper(ConfigManager.ParallelCompressionProcesses, ConfigManager.ParallelCompressionProcesses);
        }
    }

    private static string FormatFilePath(string sourceFile, State state, bool hasAlpha)
    {

        var joinMatch = Regex.Match(state.RoomInfo.JoinDateTime, @"(?<Year>\d+)\.(?<Month>\d+)\.(?<Day>\d+) (?<Hour>\d+):(?<Minute>\d+):(?<Second>\d+)");
        var takenMatch = Regex.Match(sourceFile, @"(?<Year>\d+)-(?<Month>\d+)-(?<Day>\d+)_(?<Hour>\d+)-(?<Minute>\d+)-(?<Second>\d+)\.(?<Millisecond>\d+)_(?<Width>\d+)x(?<Height>\d+)");
        var instanceType = state.RoomInfo.Permission switch
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
        var cameraType = state.VirtualLens2.Enabled ? "VirtualLens2"
                         : state.Integral.Enabled ? "Integral"
                         : "VRCCamera";

        var placeholders = new Dictionary<string, string>
        {
            { "%TAKEN:yyyy%", takenMatch.Groups["Year"].Value },
            { "%TAKEN:MM%", takenMatch.Groups["Month"].Value },
            { "%TAKEN:dd%", takenMatch.Groups["Day"].Value },
            { "%TAKEN:hh%", takenMatch.Groups["Hour"].Value },
            { "%TAKEN:mm%", takenMatch.Groups["Minute"].Value },
            { "%TAKEN:ss%", takenMatch.Groups["Second"].Value },
            { "%TAKEN:fff%", takenMatch.Groups["Millisecond"].Value },

            { "%JOIN:yyyy%", joinMatch.Groups["Year"].Value },
            { "%JOIN:MM%", joinMatch.Groups["Month"].Value },
            { "%JOIN:dd%", joinMatch.Groups["Day"].Value },
            { "%JOIN:hh%", joinMatch.Groups["Hour"].Value },
            { "%JOIN:mm%", joinMatch.Groups["Minute"].Value },
            { "%JOIN:ss%", joinMatch.Groups["Second"].Value },

            { "yyyy", takenMatch.Groups["Year"].Value },
            { "MM", takenMatch.Groups["Month"].Value },
            { "dd", takenMatch.Groups["Day"].Value },
            { "hh", takenMatch.Groups["Hour"].Value },
            { "mm", takenMatch.Groups["Minute"].Value },
            { "ss", takenMatch.Groups["Second"].Value },
            { "fff", takenMatch.Groups["Millisecond"].Value },
            { "XXXX", takenMatch.Groups["Width"].Value },
            { "YYYY", takenMatch.Groups["Height"].Value },

            { "%WORLD%", string.IsNullOrEmpty(state.RoomInfo.World_name) ? "UNKNOWN WORLD" : state.RoomInfo.World_name },
            { "%WORLD:ID%", state.RoomInfo.World_id },
            { "%INSTANCE:ID%", state.RoomInfo.Instance_id},
            { "%INSTANCE:TYPE%", instanceType  },
            { "%OWNER:ID%", state.RoomInfo.Organizer },

            { "%CAMERA%", cameraType}
        };

        var filePath = hasAlpha ? ConfigManager.AlphaFilePattern : ConfigManager.FilePattern;

        foreach (var entry in placeholders)
        {
            filePath = filePath.Replace(entry.Key, entry.Value);
        }

        return filePath;
    }

    public static void Process(string sourcePath, State state)
    {
        if (!new FileInfo(sourcePath).Exists) return;

        var hasAlpha = false;
        {
            using var targetImage = new Bitmap(sourcePath);

            var formatWithAlpha = new PixelFormat[] { PixelFormat.Alpha, PixelFormat.Canonical, PixelFormat.Format16bppArgb1555, PixelFormat.Format32bppArgb, PixelFormat.Format32bppPArgb, PixelFormat.Format64bppArgb, PixelFormat.Format64bppPArgb };
            if (formatWithAlpha.Contains(targetImage.PixelFormat))
            {
                hasAlpha = true;
            }
        }

        var filePath = FormatFilePath(Path.GetFileName(sourcePath), state, hasAlpha);

        filePath = Regex.Replace(filePath, @"[<>:""|?*]", "_");

        var destPath = ConfigManager.DestDir;
        if (destPath == "")
        {
            var sourceDir = Path.GetDirectoryName(sourcePath);
            if (sourceDir is null)
                return;

            destPath = new DirectoryInfo(sourceDir)?.Parent?.FullName;
            if (destPath is null)
                return;
        }

        var basePath = Path.GetFullPath(destPath);

        destPath = Path.Combine(destPath, filePath);

        if (!Path.GetFullPath(destPath).StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var destDir = Path.GetDirectoryName(destPath);

        if (destDir is null)
            return;

        if (Directory.CreateDirectory(destDir).Exists == false)
        {
            UI.SendNotify.Send(Properties.Resources.NotifyErrorImageProcessCantCreateDirectory, false);
            return;
        }

        if (!ConfigManager.OverwriteDestinationFile && new FileInfo(destPath).Exists)
        {
            UI.SendNotify.Send(Properties.Resources.NotifyErrorImageProcessFileExist, false);
            return;
        }
        using (s_compressSemaphore?.Wait())
        {
            var tmpPath = Compress(sourcePath, hasAlpha);
            if (new FileInfo(tmpPath).Exists)
            {
                if (WriteMetadata(tmpPath, destPath, state) == true && ConfigManager.DeleteOriginalFile)
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
            }
        }
    }

    private static string Compress(string sourcePath, bool hasAlpha)
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

        args.AddRange(StateChecker.VirtualLens2.Publish(state.VirtualLens2));
        args.AddRange(StateChecker.Integral.Publish(state.Integral));

        if (!(state.VirtualLens2.Enabled || state.Integral.Enabled))
        {
            args.Add("-:Make=VRChat");
            args.Add("-:Model=VRChat Camera");
        }

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

}
