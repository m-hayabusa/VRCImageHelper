namespace VRCImageHelper;

using System.Text.Json;
using System.Text.Json.Serialization;

internal class ConfigManager
{
    public static Config Config { get; private set; } = Load();
    public static void Save(Config config)
    {
        var path = $"{Path.GetDirectoryName(Application.ExecutablePath)}\\config.json";
        var result = JsonSerializer.Serialize(config);

        var confDir = Path.GetDirectoryName(path);
        if (!Directory.Exists(confDir) && confDir is not null)
        {
            Directory.CreateDirectory(confDir);
        }

        File.WriteAllText(path, result);

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

        return result;
    }
}

internal class Config
{
    public static Config Default { get; } = new();
    public string DestDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\VRCImage";
    public string FilePattern { get; set; } = "yyyy-MM\\VRChat_yyyy-MM-dd_hh-mm-ss.fff_XXXXxYYYY.png";
    public string Format { get; set; } = "PNG";
    public string Encoder { get; set; } = "libaom-av1";
    public string EncoderOption { get; set; } = "";
    public int Quality { get; set; } = 20;
    public VirtualLens2Config VirtualLens2 { get; set; } = new();
    internal class VirtualLens2Config
    {
        public float ApertureMin { get; set; } = 22;
        public float ApertureMax { get; set; } = 1;
        public float ApertureDefault { get; set; } = 22;
        public float FocalLengthMin { get; set; } = 12;
        public float FocalLengthMax { get; set; } = 300;
        public float FocalLengthDefault { get; set; } = 50;
    }

    [JsonConstructor]
    public Config(string destDir, string filePattern, string format, string encoder, string encoderOption, int quality, VirtualLens2Config virtualLens2)
    {
        DestDir = destDir;
        FilePattern = filePattern;
        Format = format;
        Encoder = encoder;
        EncoderOption = encoderOption;
        Quality = quality;
        VirtualLens2 = virtualLens2;
    }
    public Config() { }
}
