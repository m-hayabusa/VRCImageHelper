namespace VRCImageHelper.Core;

using System.Text.Json;
using VRCImageHelper.Tools;

internal class ConfigManager
{
    public static Config GetConfig()
    {
        return s_config.Clone();
    }
    private static Config s_config = Load();
    public static bool ScanAll { get { return s_config.ScanAll; } }
    public static bool OverwriteDestinationFile { get { return s_config.OverwriteDestinationFile; } }
    public static bool DeleteOriginalFile { get { return s_config.DeleteOriginalFile; } }
    public static string DestDir { get { return s_config.DestDir; } }
    public static string FilePattern { get { return s_config.FilePattern; } }
    public static string Format { get { return s_config.Format; } }
    public static string Encoder { get { return s_config.Encoder; } }
    public static string EncoderOption { get { return s_config.EncoderOption; } }
    public static int Quality { get { return s_config.Quality; } }
    public static string AlphaFilePattern { get { return s_config.AlphaFilePattern; } }
    public static string AlphaFormat { get { return s_config.AlphaFormat; } }
    public static string AlphaEncoder { get { return s_config.AlphaEncoder; } }
    public static string AlphaEncoderOption { get { return s_config.AlphaEncoderOption; } }
    public static int AlphaQuality { get { return s_config.AlphaQuality; } }

    internal static class VirtualLens2
    {
        public static float ApertureMin { get { return s_config.VirtualLens2.ApertureMin; } }
        public static float ApertureMax { get { return s_config.VirtualLens2.ApertureMax; } }
        public static float ApertureDefault { get { return s_config.VirtualLens2.ApertureDefault; } }
        public static float FocalLengthMin { get { return s_config.VirtualLens2.FocalLengthMin; } }
        public static float FocalLengthMax { get { return s_config.VirtualLens2.FocalLengthMax; } }
        public static float FocalLengthDefault { get { return s_config.VirtualLens2.FocalLengthDefault; } }
    }

    public static void Save(Config config)
    {
        var path = $"{Path.GetDirectoryName(Application.ExecutablePath)}\\config.json";

        if (config.VirtualLens2.ApertureDefault > config.VirtualLens2.ApertureMin)
            config.VirtualLens2.ApertureDefault = config.VirtualLens2.ApertureMin;

        var result = JsonSerializer.Serialize(config);

        var confDir = Path.GetDirectoryName(path);
        if (!Directory.Exists(confDir) && confDir is not null)
        {
            Directory.CreateDirectory(confDir);
        }

        File.WriteAllText(path, result);

        if (config.VirtualLens2.ApertureDefault >= config.VirtualLens2.ApertureMin)
            config.VirtualLens2.ApertureDefault = float.PositiveInfinity;

        s_config = config;
    }

    public static Config Load()
    {
        var path = $"{Path.GetDirectoryName(Application.ExecutablePath)}\\config.json";
        var result = new Config();

        if (File.Exists(path))
        {
            var source = File.ReadAllText(path);
            result = JsonSerializer.Deserialize<Config>(source) ?? result;
        }

        if ((result.Format == "AVIF" && FFMpeg.GetSupportedEncoder("av1").Length == 0) || (result.Format == "WEBP" && FFMpeg.GetSupportedEncoder("webp").Length == 0))
        {
            result.Format = Config.Default.Format;
            result.FilePattern = Path.ChangeExtension(result.FilePattern, result.Format.ToLower());
        }
        if ((result.Format == "AVIF" && !FFMpeg.GetSupportedEncoder("av1").Contains(result.Encoder)) || result.Format == "WEBP" && !FFMpeg.GetSupportedEncoder("webp").Contains(result.Encoder))
        {
            result.Encoder = Config.Default.Encoder;
        }

        if (result.VirtualLens2.ApertureDefault >= result.VirtualLens2.ApertureMin)
            result.VirtualLens2.ApertureDefault = float.PositiveInfinity;

        return result;
    }
    public static string DefaultEncoderOptions(string encoder, bool hasAlphaChannel)
    {
        return encoder switch
        {
            "" => "",
            "libaom-av1" => "-r 1 -still-picture 1" + (hasAlphaChannel ? "-filter:v:1 alphaextract -map 0 -map 0" : ""),
            "av1_qsv" => "-r 1 -preset veryslow",
            "libwebp" => "-preset picture",
            _ => "-r 1",
        };
    }
}

internal class Config
{
    public Config Clone()
    {
        var clone = (Config)MemberwiseClone();
        clone.VirtualLens2 = VirtualLens2.Clone();
        return clone;
    }
    public static Config Default { get; } = new();
    public bool ScanAll { get; set; } = false;
    public bool OverwriteDestinationFile { get; set; } = false;
    public bool DeleteOriginalFile { get; set; } = false;
    public string DestDir { get; set; } = "";
    public string FilePattern { get; set; } = "yyyy-MM\\VRChat_yyyy-MM-dd_hh-mm-ss.fff_XXXXxYYYY.png";
    public string Format { get; set; } = "PNG";
    public string Encoder { get; set; } = "";
    public string EncoderOption { get; set; } = "";
    public int Quality { get; set; } = 20;
    public string AlphaFilePattern { get; set; } = "yyyy-MM\\VRChat_yyyy-MM-dd_hh-mm-ss.fff_XXXXxYYYY.png";
    public string AlphaFormat { get; set; } = "PNG";
    public string AlphaEncoder { get; set; } = "";
    public string AlphaEncoderOption { get; set; } = "";
    public int AlphaQuality { get; set; } = 20;
    public VirtualLens2Config VirtualLens2 { get; set; } = new();
    internal class VirtualLens2Config
    {
        public VirtualLens2Config Clone()
        {
            return (VirtualLens2Config)MemberwiseClone();
        }
        public float ApertureMin { get; set; } = 22;
        public float ApertureMax { get; set; } = 1;
        public float ApertureDefault { get; set; } = float.PositiveInfinity;
        public float FocalLengthMin { get; set; } = 12;
        public float FocalLengthMax { get; set; } = 300;
        public float FocalLengthDefault { get; set; } = 50;
    }
}
