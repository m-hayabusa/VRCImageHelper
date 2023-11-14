namespace VRCImageHelper.Core;

using System.Text.Json;
using VRCImageHelper.Tools;

internal class ConfigManager
{
    public static Config Config { get; private set; } = Load();

    public static string DestDir { get { return Config.DestDir; } }
    public static string FilePattern { get { return Config.FilePattern; } }
    public static string Format { get { return Config.Format; } }
    public static string Encoder { get { return Config.Encoder; } }
    public static string EncoderOption { get { return Config.EncoderOption; } }
    public static int Quality { get { return Config.Quality; } }
    public static string AlphaFilePattern { get { return Config.AlphaFilePattern; } }
    public static string AlphaFormat { get { return Config.AlphaFormat; } }
    public static string AlphaEncoder { get { return Config.AlphaEncoder; } }
    public static string AlphaEncoderOption { get { return Config.AlphaEncoderOption; } }
    public static int AlphaQuality { get { return Config.AlphaQuality; } }

    internal static class VirtualLens2
    {
        public static float ApertureMin { get { return Config.VirtualLens2.ApertureMin; } }
        public static float ApertureMax { get { return Config.VirtualLens2.ApertureMax; } }
        public static float ApertureDefault { get { return Config.VirtualLens2.ApertureDefault; } }
        public static float FocalLengthMin { get { return Config.VirtualLens2.FocalLengthMin; } }
        public static float FocalLengthMax { get { return Config.VirtualLens2.FocalLengthMax; } }
        public static float FocalLengthDefault { get { return Config.VirtualLens2.FocalLengthDefault; } }
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

        Config = config;
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

        if (result.Format == "AVIF" && FFMpeg.GetSupportedEncoder("av1").Length == 0)
        {
            result.Format = Config.Default.Format;
            result.FilePattern = Path.ChangeExtension(result.FilePattern, result.Format.ToLower());
        }
        if (result.Format == "AVIF" && !FFMpeg.GetSupportedEncoder("av1").Contains(result.Encoder))
        {
            result.Encoder = Config.Default.Encoder;
        }

        if (result.VirtualLens2.ApertureDefault >= result.VirtualLens2.ApertureMin)
            result.VirtualLens2.ApertureDefault = float.PositiveInfinity;

        return result;
    }
}

internal class Config
{
    public static Config Default { get; } = new();
    public string DestDir { get; set; } = "";
    public string FilePattern { get; set; } = "yyyy-MM\\VRChat_yyyy-MM-dd_hh-mm-ss.fff_XXXXxYYYY.png";
    public string Format { get; set; } = "PNG";
    public string Encoder { get; set; } = "libaom-av1";
    public string EncoderOption { get; set; } = "-r 1 -preset veryslow -still-picture 1";
    public int Quality { get; set; } = 20;
    public string AlphaFilePattern { get; set; } = "yyyy-MM\\VRChat_yyyy-MM-dd_hh-mm-ss.fff_XXXXxYYYY.png";
    public string AlphaFormat { get; set; } = "PNG";
    public string AlphaEncoder { get; set; } = "libaom-av1";
    public string AlphaEncoderOption { get; set; } = "-r 1 -preset veryslow -still-picture 1 -filter:v:1 alphaextract -map 0 -map 0";
    public int AlphaQuality { get; set; } = 20;
    public VirtualLens2Config VirtualLens2 { get; set; } = new();
    internal class VirtualLens2Config
    {
        public float ApertureMin { get; set; } = 22;
        public float ApertureMax { get; set; } = 1;
        public float ApertureDefault { get; set; } = float.PositiveInfinity;
        public float FocalLengthMin { get; set; } = 12;
        public float FocalLengthMax { get; set; } = 300;
        public float FocalLengthDefault { get; set; } = 50;
    }
}
