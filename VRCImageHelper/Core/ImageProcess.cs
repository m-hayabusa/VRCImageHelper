namespace VRCImageHelper.Core;

using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Timers;
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

internal struct QueueTask
{
    public QueueTask(string path)
    {
        this.path = path;
    }
    public void SetIsProcessing(bool isProcessing)
    {
        this.isProcessing = isProcessing;
    }
    public void SetState(State state)
    {
        this.state = state;
    }
    public string path;
    public bool isProcessing = false;
    public State? state = null;
}

internal static class ProcessQueue
{
    public static SemaphoreSlimWrapper? s_compressSemaphore;
    public static SortedDictionary<DateTime, LinkedList<QueueTask>> s_queue = new();
    private static readonly Timer s_timer;

    static ProcessQueue()
    {
        s_timer = new Timer(10000);
        s_timer.Elapsed += (sender, e) => CheckQueue();
        s_timer.AutoReset = false; // 一度タイムアウトしたら再実行を防ぐ

        if (ConfigManager.ParallelCompressionProcesses > 0)
        {
            s_compressSemaphore = new SemaphoreSlimWrapper(ConfigManager.ParallelCompressionProcesses, ConfigManager.ParallelCompressionProcesses);
        }
    }

    private static void ResetTimer()
    {
        s_timer.Stop();
        s_timer.Start();
    }

    public static void Enqueue(string path, State? state = null)
    {
        if (ParseDate.TryParseFilePathToDateTime(path, out var timestamp))
        {
            Debug.WriteLine($"抽出された日時: {timestamp} {path}");
            if (!s_queue.ContainsKey(timestamp))
            {
                s_queue[timestamp] = new LinkedList<QueueTask>();
            }

            if (!s_queue[timestamp].Any(item => item.path == path))
            {
                s_queue[timestamp].AddLast(new QueueTask(path) { state = state });
                Debug.WriteLine($"Task '{path}' added at {timestamp}.");
                CheckQueue();
            }
            else
            {
                Debug.WriteLine($"Task '{path}' already exists at {timestamp}, not adding.");
            }
        }
        else
        {
            Debug.WriteLine("ファイル名から日時情報を抽出できませんでした。");
        }
    }

    public static void CheckQueue()
    {
        ResetTimer();

        var currentLogTime = LogReader.CurrentHead;

        foreach (var list in s_queue.Where(file => file.Key < currentLogTime))
        {
            Debug.WriteLine("CheckQueue foreach#LIST " + list.Key + " " + list.Value.Count);

            foreach (var item in list.Value)
            {
                Debug.WriteLine("  CheckQueue foreach#ITEM " + item.path);

                if (item.isProcessing)
                    break;
                item.SetIsProcessing(true);

                var state = item.state ?? State.Current.Clone();

                state.CreationDate = list.Key.ToString("yyyy:MM:dd HH:mm:ss");

                Debug.WriteLine("キューから処理" + item);
                new Task(() =>
                {
                    using (s_compressSemaphore?.Wait())
                    {
                        Debug.WriteLine("キューから処理: 実行中" + item);
                        ImageProcess.Process(item.path, state);
                        list.Value.Remove(item);
                        if (list.Value.Count == 0)
                        {
                            s_queue.Remove(list.Key);
                        }
                    }
                }).Start();
            }
        }
    }
}

internal class ImageProcess
{
    public static void Taken(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, @"[0-9.: ]* (?:Log|Debug) +? -  \[VRC Camera\] Took screenshot to: (?<Path>.*)");
        if (match.Success)
        {
            ProcessQueue.Enqueue(match.Groups["Path"].ToString(), State.Current.Clone());
        }
    }

    private static string FormatFilePath(string sourceFile, State state, bool hasAlpha, bool isPrint)
    {
        var joinMatch = Regex.Match(state.RoomInfo.JoinDateTime, @"(?<Year>\d+)\.(?<Month>\d+)\.(?<Day>\d+) (?<Hour>\d+):(?<Minute>\d+):(?<Second>\d+)");
        var takenMatch = Regex.Match(sourceFile, @"(?<Year>\d+)-(?<Month>\d+)-(?<Day>\d+)_(?<Hour>\d+)-(?<Minute>\d+)-(?<Second>\d+)\.(?<Millisecond>\d+)_(?<Width>\d+)x(?<Height>\d+)(?<MultiLayer>_[A-Za-z]+)?");
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
        var cameraType = isPrint ? "Print"
                       : state.VirtualLens2.Enabled ? "VirtualLens2"
                       : state.Integral.Enabled ? "Integral"
                       : "VRCCamera";

        var placeholders = new Dictionary<string, string>
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

        var isPrint = false;
        var hasAlpha = false;

        {
            using var targetImage = new Bitmap(sourcePath);

            static bool CheckCornersAreWhite(Bitmap image)
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

            if (targetImage.Width == 2048 && targetImage.Height == 1440 && CheckCornersAreWhite(targetImage))
            {
                isPrint = true;
                // Printが何故か32bit深度
                hasAlpha = false;
            }
            else
            {
                var formatWithAlpha = new PixelFormat[] { PixelFormat.Alpha, PixelFormat.Canonical, PixelFormat.Format16bppArgb1555, PixelFormat.Format32bppArgb, PixelFormat.Format32bppPArgb, PixelFormat.Format64bppArgb, PixelFormat.Format64bppPArgb };
                if (formatWithAlpha.Contains(targetImage.PixelFormat))
                {
                    hasAlpha = true;
                }
            }
        }

        var filePath = FormatFilePath(Path.GetFileName(sourcePath), state, hasAlpha, isPrint);

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
        UI.SendNotify.Send("OK!", false);
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
