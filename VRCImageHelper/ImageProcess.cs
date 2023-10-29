using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text.Json;
using System.Text.RegularExpressions;
using FFMpegCore;
using NExifTool;
using NExifTool.Writer;

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
                    if (DateTime.TryParse(CreationDate, out DateTime dT))
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

        ImageProcess(string path, State state) {
            State = state;
            Path = path;
        }

        private void Process()
        {
            if (new FileInfo(Path).Exists)
            {
                var tmpPath = Compress(Path, "avif", 10);
                var destPath = tmpPath;

                if (!new FileInfo(tmpPath).Exists)
                {
                    WriteMetadata(tmpPath, destPath, State);
                }
            }
        }

        private static string Compress(string path, string encode, int quality)
        {
            var dest = path.Remove(path.Length - 3) + encode;
            if (!(new FileInfo(dest).Exists))
            {
                switch (encode)
                {
                    case "avif":
                        CompressAVIF(path, dest, quality);
                        return dest;

                    case "jpeg":
                        CompressJPEG(path, dest, 100 - quality);
                        return dest;
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
            FFMpegArguments
                .FromFileInput(src)
                .OutputToFile(dest, false, options =>
                {
                    options
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
            var desc = $"Taken at {state.RoomInfo.World_name} ({state.RoomInfo.Organizer}'s {state.RoomInfo.Permission}), with {string.Join(",", state.Players)}.";

            Debug.WriteLine("Write " + path + " -> " + destPath);

            var tags = new List<Operation>()
            {
                new SetOperation(new Tag("CreationDateOriginal", state.CreationDate)),
                new SetOperation(new Tag("CreationTime", state.CreationDate)),
                new SetOperation(new Tag("ImageDescription", desc)),
                new SetOperation(new Tag("Description", desc)),
                new SetOperation(new Tag("MakerNote", JsonSerializer.Serialize(state)))
            };

            var exifTool = new ExifTool(new ExifToolOptions { });
            exifTool.OverwriteTagsAsync(destPath, tags, FileWriteMode.OverwriteOriginal);
        }

    }
}
