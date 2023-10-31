namespace VRCImageHelper;

using FFMpegCore;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

internal class ImageProcess
{
    private readonly string _sourcePath;
    private readonly State _state;

    private ImageProcess(string path, State state)
    {
        _state = state;
        _sourcePath = path;
    }

    public static void Taken(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, "([0-9\\.\\: ]*) Log        -  \\[VRC Camera\\] Took screenshot to\\: (.*)");
        if (match.Success)
        {
            var state = Info.State;

            var creationDate = match.Groups[1].ToString().Replace('.', ':');
            if (DateTime.TryParseExact(creationDate, "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out var dT))
            {
                creationDate += $"+{TimeZoneInfo.Local.GetUtcOffset(dT)}";
            }
            state.CreationDate = creationDate;

            var path = match.Groups[2].ToString();

            var t = new Task(new ImageProcess(path, state).Process);
            t.Start();
        }
    }

    private void Process()
    {
        var fileName = Path.GetFileName(_sourcePath);

        var match = Regex.Match(fileName, "(\\d+)-(\\d+)-(\\d+)_(\\d+)-(\\d+)-(\\d+)\\.(\\d+)_(\\d+)x(\\d+)");
        if (match.Success)
        {
            fileName = ConfigManager.Config.FilePattern
                .Replace("yyyy", match.Groups[1].Value)
                .Replace("MM", match.Groups[2].Value)
                .Replace("dd", match.Groups[3].Value)
                .Replace("hh", match.Groups[4].Value)
                .Replace("mm", match.Groups[5].Value)
                .Replace("ss", match.Groups[6].Value)
                .Replace("fff", match.Groups[7].Value)
                .Replace("XXXX", match.Groups[8].Value)
                .Replace("YYYY", match.Groups[9].Value);

            fileName = Path.ChangeExtension(fileName, ConfigManager.Config.Format.ToLower());
        }

        var destPath = ConfigManager.Config.DestDir + "\\" + fileName;
        var destDir = Path.GetDirectoryName(destPath);

        if (destDir is null)
            return;

        if (Directory.CreateDirectory(destDir).Exists == false)
            return;

        if (new FileInfo(_sourcePath).Exists && !new FileInfo(destPath).Exists)
        {
            var tmpPath = Compress(_sourcePath, ConfigManager.Config.Format, ConfigManager.Config.Quality);
            if (new FileInfo(tmpPath).Exists)
            {
                WriteMetadata(tmpPath, destPath, _state);
            }
        }
    }

    private static string Compress(string sourcePath, string encode, int quality)
    {
        var destPath = Path.GetTempFileName();
        File.Delete(destPath);

        switch (encode)
        {
            case "AVIF":
                CompressAVIF(sourcePath, destPath, quality);
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
                    switch (ConfigManager.Config.Encoder)
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

                    if (ConfigManager.Config.EncoderOption != "")
                        options.WithCustomArgument(ConfigManager.Config.EncoderOption);
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

        args += $"-overwrite_original -codedcharacterset=utf8\n";
        args += $"-:CreationTime={state.CreationDate}\n";
        args += $"-:DateTimeOriginal={state.CreationDate}\n";
        args += $"-:ImageDescription={desc}\n";
        args += $"-:Description={desc}\n";
        args += $"-:Comment={desc}\n";
        args += $"-makernote={makernote}\n";
        args += "-sep \";\"\n";
        args += $"-:Keywords={state.RoomInfo.World_name};{string.Join(';', state.Players)}\n";

        if (state.VL2Enabled)
        {
            args += $"-:Make=logilabo\n";
            args += $"-:Model=VirtualLens2\n";
            args += $"-:FocalLength={state.FocalLength}\n";
            args += $"-:FNumber={state.ApertureValue}\n";
        }
        else
        {
            args += $"-:Make=VRChat\n";
            args += $"-:Model=VRChat Camera\n";
        }

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
