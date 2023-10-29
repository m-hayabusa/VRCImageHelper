using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using FFMpegCore;

namespace VRCImageHelper
{
    internal class ImageProcess
    {
        public static void Taken(object sender, NewLineEventArgs e)
        {
            var match = Regex.Match(e.Line, "([0-9\\.\\: ]*) Log        -  \\[VRC Camera\\] Took screenshot to\\: (.*)");
            if (match.Success)
            {
                State state = Info.State;

                {
                    var CreationDate = match.Groups[1].ToString().Replace('.', ':');

                    if (DateTime.TryParseExact(CreationDate, "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime dT))
                    {
                        CreationDate += $"+{TimeZoneInfo.Local.GetUtcOffset(dT)}";
                    }
                    state.CreationDate = CreationDate;
                }

                var path = match.Groups[2].ToString();

                var t = new Task(new ImageProcess(path, state).Process);
                t.Start();
            }
        }

        private readonly string Path;
        private readonly State State;

        ImageProcess(string path, State state)
        {
            State = state;
            Path = path;
        }

        private void Process()
        {
            if (new FileInfo(Path).Exists)
            {
                var tmpPath = Compress(Path, "avif", 10);
                var destPath = Path.Remove(Path.Length - 3) + "avif";

                if (new FileInfo(tmpPath).Exists)
                {
                    WriteMetadata(tmpPath, destPath, State);
                }
            }
        }

        private static string Compress(string path, string encode, int quality)
        {
            var dest = System.IO.Path.GetTempFileName();
            File.Delete(dest);

            if (!(new FileInfo(dest).Exists))
            {
                switch (encode)
                {
                    case "avif":
                        CompressAVIF(path, dest, quality);
                        break;

                    case "jpeg":
                        CompressJPEG(path, dest, 100 - quality);
                        break;

                    case "png":
                        File.Copy(path, dest);
                        break;
                }
            }

            return dest;
        }

        private static void CompressJPEG(string src, string dest, int quality)
        {
            var image = new Bitmap(src);
            var encoder = ImageCodecInfo.GetImageEncoders().ToList()
                            .Where(e => e.FormatID == ImageFormat.Jpeg.Guid)
                            .First();

            var encodeParams = new EncoderParameters(1);
            encodeParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            image.Save(dest, encoder, encodeParams);
        }

        private static void CompressAVIF(string src, string dest, int quality)
        {
            Debug.WriteLine(dest);
            FFMpegArguments
                .FromFileInput(src)
                .OutputToFile(dest, false, options =>
                {
                    options
                        .ForceFormat("avif")
                        .WithSpeedPreset(FFMpegCore.Enums.Speed.VerySlow)
                        .WithCustomArgument("-still-picture 1")
                        .WithFramerate(1);

                    var formatWithAlpha = new PixelFormat[] { PixelFormat.Alpha, PixelFormat.Canonical, PixelFormat.Format16bppArgb1555, PixelFormat.Format32bppArgb, PixelFormat.Format32bppPArgb, PixelFormat.Format64bppArgb, PixelFormat.Format64bppPArgb };
                    if (formatWithAlpha.Contains((new Bitmap(src)).PixelFormat))
                    {
                        options
                            .WithVideoCodec("libaom-av1")
                            .WithConstantRateFactor(quality)
                            .WithCustomArgument("-filter:v:1 alphaextract")
                            .WithCustomArgument("-map 0")
                            .WithCustomArgument("-map 0");
                    }
                    else
                    {
                        string encoder = "av1_qsv";
                        switch (encoder)
                        {
                            case "libaom-av1":
                                options
                                    .WithVideoCodec("libaom-av1")
                                    .WithConstantRateFactor(quality);
                                break;
                            case "av1_qsv":
                                options
                                    .WithVideoCodec("av1_qsv")
                                    .WithCustomArgument($"-q {quality}");
                                break;
                            case "av1_nvenc":
                                options
                                    .WithVideoCodec("av1_nvenc");
                                break;
                            case "av1_amf":
                                options
                                    .WithVideoCodec("av1_amf");
                                break;
                        }
                    }
                })
                .ProcessSynchronously();
        }

        private static void WriteMetadata(string path, string destPath, State state)
        {
            var desc = $"Taken at {state.RoomInfo.World_name}, with {string.Join(",", state.Players)}.";

            var makernote = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(state, options: new JsonSerializerOptions(JsonSerializerDefaults.Web) { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping })
                )
            );

            var argsFilePath = System.IO.Path.GetTempFileName();
            var args = "";

            args += $"-overwrite_original\n";
            args += $"-:CreationTime={state.CreationDate}\n";
            args += $"-:DateTimeOriginal={state.CreationDate}\n";
            args += $"-:ImageDescription={desc}\n";
            args += $"-:Description={desc}\n";
            args += $"-:Comment={desc}\n";
            args += $"-makernote={makernote}\n";

            var argsFile = new StreamWriter(argsFilePath);
            argsFile.Write(args);
            argsFile.Dispose();

            Debug.WriteLine(args);

            var exifTool = new ProcessStartInfo("exiftool.exe") { Arguments = path + " -@ " + argsFilePath, CreateNoWindow = true };
            var exifToolProcess = System.Diagnostics.Process.Start(exifTool);
            if (exifToolProcess is not null)
            {
                exifToolProcess.WaitForExit();
                File.Delete(destPath);
                File.Move(path, destPath);
                File.Delete(argsFilePath);
            }
        }
    }
}
