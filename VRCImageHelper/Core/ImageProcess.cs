namespace VRCImageHelper.Core;

using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
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

    public static void CheckQueue(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, @"$\d{4}.\d{2}.\d{2} \d{2}:\d{2}:\d{2}");

        if (match.Success)
        {
            // ログファイルから見たときでも、書き込み先パスがわかる → ファイル名から同じ時刻が計算できる → キーが同じであることでディレクトリ監視で見つけたものとの重複を検知できるはず (キーは時刻)
            // 「処理済みのリスト」は持つ必要ない
            // 「処理中のリスト」は必要 (ログとディレクトリ監視両方でキューに積みうる)
            //   キューから落とすタイミングを処理完了時にすればいい？そんなことはないな (この関数が処理中にも呼ばれる)
            // どこまで処理したかを持てば、それより古いファイルは処理済みであることにできる
            // 「どこまで処理したか」、すなわち最後に処理が完了したファイル名から計算される時刻であり、これを永続化することでプログラムの再起動時に重複処理をしないようにできる

            // 「過去ログをすべてスキャン」時は、ファイル一覧を取ってキューに積み、ログファイルを見て、ログ上を舐めるカーソルの時刻がファイル名から計算される時刻を越えたときに処理を実行する
            //   これだけだと、ログ先端でそれより過去のものすべてを処理してしまうので、それをケアする必要がある

            // まとめると

            // 初期化
            //   ログファイルの先頭位置がCurrentHeadに保持される (のを、待つ → 2回目のlogReader.NewLine) // done
            //     それと同じ月か、それより新しいディレクトリ内にある、それより新しいすべてのファイルをキューに持つ // done -> 変更
            // 監視
            //   ディレクトリ監視で、新しく生えたもの、キューに積む // done -> 変更
            //     キューに積むとき、同じキーで同じファイル名のものがあれば、積まない // done
            //   ログファイル監視で、新しく生えたもの、キューに積む // WIP
            //     キューに積むとき、同じキーで同じファイル名のものがあれば、積まない
            //     積んだあと、TakenからLogChangedを呼び出す (実質的に即時処理)
            // 処理
            //   logReader.NewLineで、キューからキーがログの時刻よりも古いものを抽出し、Processする // done
            //     キューに追加する処理はImageProcess.Enqueueにして、キュー自体もこちらで持つ
            //       Enqueue時、CheckQueueも呼ぶ
            //       ログの先頭かどうかはどうでもよくて、CurrentHeadを見ればいいので、CheckQueueは「15秒ごと、あるいはEnqueueから」呼べばいい
            //   Processでは、処理中リストにファイル名を積んでから処理する
            //     処理が完了したら、キューから削除して、処理済みの時刻が自分よりも古ければ、それを更新して、積んだファイル名を処理中リストから落とす

            var currentLogTime = DateTime.ParseExact(match.Value, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);

            // この処理タイマーでも呼ぶので別関数に切り出す
            foreach (var item in FileWatcher.s_queue.Where(file => file.Key > currentLogTime))
            {
                Debug.WriteLine("キューから処理" + item.Value);
                var state = State.Current.Clone();

                state.CreationDate = item.Key.ToString("yyyy:MM:dd HH:mm:ss");

                new Task(() => Process(item.Value, state)).Start();
            }
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
        var isPrint = takenMatch.Groups["Width"].Value == "2048" && takenMatch.Groups["Height"].Value == "1440";
        var cameraType = state.VirtualLens2.Enabled ? "VirtualLens2"
                         : state.Integral.Enabled ? "Integral"
                         : isPrint ? "Print"
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

        var hasAlpha = false;
        {
            using var targetImage = new Bitmap(sourcePath);

            if (targetImage.Width == 2048 && targetImage.Height == 1440)
            {
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
