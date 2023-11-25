namespace VRCImageHelper.Core;

using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

internal class RoomInfo
{
    public RoomInfo() { }
    public RoomInfo(RoomInfo roomInfo)
    {
        World_id = roomInfo.World_id;
        World_name = roomInfo.World_name;
        Permission = roomInfo.Permission;
        Organizer = roomInfo.Organizer;
    }
    public string? World_id { get; set; }
    public string? World_name { get; set; }
    public string? Permission { get; set; }
    public string? Organizer { get; set; }
}

internal class IntegralExposureParam
{
    public IntegralExposureParam()
    {

        FocalLength = ConfigManager.Integral.FocalLengthDefault;
        ApertureValue = (FocalLength / 1000) / (2.0f * ConfigManager.Integral.ApertureDefault);
        ExposureTime = 0;
        ExposureBias = ConfigManager.Integral.ExposureDefault;
        LensModel = ConfigManager.Integral.BokehShapeNames[ConfigManager.Integral.BokehShapeDefault];
    }
    public IntegralExposureParam(IntegralExposureParam IntegralShotState)
    {
        FocalLength = IntegralShotState.FocalLength;
        ApertureValue = IntegralShotState.ApertureValue;
        ExposureTime = IntegralShotState.ExposureTime;
        ExposureBias = IntegralShotState.ExposureBias;
        LensModel = IntegralShotState.LensModel;
    }
    public float FocalLength { get; set; }
    public float ApertureValue { get; set; }
    public float ExposureBias { get; set; }
    public string LensModel { get; set; }
    public float ExposureTime { get; set; }
}
internal class IntegralParam
{
    public IntegralParam()
    {
        FocalLength = ConfigManager.Integral.FocalLengthDefault;
        ApertureSize = ConfigManager.Integral.ApertureDefault;
        ExposureTime = ConfigManager.Integral.ExposureTimeDefault;
        ExposureBias = ConfigManager.Integral.ExposureDefault;
        BokehShape = ConfigManager.Integral.BokehShapeDefault;
    }
    public IntegralParam(IntegralParam IntegralParam)
    {
        FocalLength = IntegralParam.FocalLength;
        ApertureSize = IntegralParam.ApertureSize;
        ExposureTime = IntegralParam.ExposureTime;
        ExposureBias = IntegralParam.ExposureBias;
        BokehShape = IntegralParam.BokehShape;
    }
    public float FocalLength { get; set; }
    public float ApertureSize { get; set; }
    public float ExposureTime { get; set; }
    public float ExposureBias { get; set; }
    public int BokehShape { get; set; }
    public DateTime? ExposureStartTime { get; set; }
}

internal class VirtualLens2Param
{
    public VirtualLens2Param()
    {
        FocalLength = ConfigManager.VirtualLens2.FocalLengthDefault;
        ApertureValue = ConfigManager.VirtualLens2.ApertureDefault;
    }
    public VirtualLens2Param(VirtualLens2Param virtualLens2Param)
    {
        FocalLength = virtualLens2Param.FocalLength;
        ApertureValue = virtualLens2Param.ApertureValue;
    }
    public float FocalLength { get; set; }
    public float ApertureValue { get; set; }
}

internal class State
{
    public State()
    {
        VL2Enabled = false;
        VirtualLens2 = new VirtualLens2Param();
        IntegralEnabled = false;
        CreationDate = "";
        Players = new List<string>();
        RoomInfo = new RoomInfo();
        Integral = new IntegralParam();
        IntegralExposureParams = new List<IntegralExposureParam>();
    }

    public State Clone()
    {
        var clone = (State)MemberwiseClone();
        clone.Players = new List<string>(Players);
        clone.RoomInfo = new RoomInfo(RoomInfo);
        clone.VirtualLens2 = new VirtualLens2Param(VirtualLens2);
        clone.Integral = new IntegralParam(Integral);
        clone.IntegralExposureParams = new List<IntegralExposureParam>(IntegralExposureParams);
        return clone;
    }
    [JsonIgnore]
    public string CreationDate { get; set; }
    [JsonIgnore]
    public bool VL2Enabled { get; set; }
    [JsonIgnore]
    public bool IntegralEnabled { get; set; }
    [JsonIgnore]
    public VirtualLens2Param VirtualLens2 { get; set; }
    [JsonIgnore]
    public IntegralParam Integral { get; set; }
    // Integralは複数回露光できるため、露光時のパラメーターを記録する
    [JsonIgnore]
    public List<IntegralExposureParam> IntegralExposureParams { get; set; }
    public List<string> Players { get; set; }
    [JsonPropertyName("roominfo")]
    public RoomInfo RoomInfo { get; set; }
}

internal class StateChecker
{
    public static State State { get; private set; } = new State();

    public static void WorldId(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, ".*\\[Behaviour\\] Joining (wrld_.*?):(?:.*?(private|friends|hidden|group)\\((.*?)\\))?(~canRequestInvite|~groupAccessType\\(plus\\))?");
        if (match.Success)
        {
            Debug.WriteLine($"Joining {match.Groups[1]}, {match.Groups[2]}, {match.Groups[3]}, {match.Groups[4]}");
            State.RoomInfo.World_id = match.Groups[1].Value;
            State.RoomInfo.Permission = match.Groups[2].Value + (match.Groups[4].Success ? "+" : "");
            State.RoomInfo.Organizer = match.Groups[3].Value;
            State.Players.Clear();
        }
    }

    public static void Taken(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, "([0-9\\.\\: ]*) Log        -  \\[VRC Camera\\] Took screenshot to\\: (.*)");
        if (match.Success)
        {
            var state = State.Clone();

            var creationDate = match.Groups[1].ToString().Replace('.', ':');
            state.CreationDate = creationDate;

            var path = match.Groups[2].ToString();

            new Task(() => ImageProcess.Process(path, state)).Start();
        }
    }

    public static void JoinRoom(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, "Joining or Creating Room: (.*)");
        if (match.Success)
        {
            Debug.WriteLine($"Joining {match.Groups[1]}");
            State.RoomInfo.World_name = match.Groups[1].Value;
        }
    }

    public static void PlayerJoin(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, "OnPlayerJoined (.*)");
        if (match.Success)
        {
            Debug.WriteLine($"Join {match.Groups[1]}");
            State.Players.Add(match.Groups[1].Value);
        }
    }

    public static void PlayerLeft(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, "OnPlayerLeft (.*)");
        if (match.Success)
        {
            Debug.WriteLine($"Left {match.Groups[1]}");
            _ = State.Players.Remove(match.Groups[1].Value);
        }
    }

    public static void Quit(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, "VRCApplication: OnApplicationQuit");
        if (match.Success)
        {
            Debug.WriteLine("Quit");
            State = new State();
        }
    }

    public static void VL2Zoom(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/VirtualLens2_Zoom")
        {
            var raw = float.Parse(e.Data.Trim()[..^1]);
            var min = ConfigManager.VirtualLens2.FocalLengthMin;
            var max = ConfigManager.VirtualLens2.FocalLengthMax;
            State.VirtualLens2.FocalLength = min * MathF.Exp(raw * MathF.Log(max / min));
        }
    }

    public static void VL2Enable(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/VirtualLens2_Enable")
        {
            State.VL2Enabled = int.Parse(e.Data) == 1;
        }
    }

    public static void VL2Aperture(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/VirtualLens2_Aperture")
        {
            var raw = float.Parse(e.Data.Trim()[..^1]);
            var min = ConfigManager.VirtualLens2.ApertureMin;
            var max = ConfigManager.VirtualLens2.ApertureMax;
            if (raw == 0)
                State.VirtualLens2.ApertureValue = float.PositiveInfinity;
            else
                State.VirtualLens2.ApertureValue = min * MathF.Exp(raw * MathF.Log(max / min));
        }
    }

    private static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    private static float CalcBlend(float[] values, float raw)
    {
        raw = raw * (values.Length - 1);
        return Lerp(values[(int)raw], values[(int)MathF.Min(raw + 1, values.Length - 1)], raw - ((int)raw));
    }

    public static void IntegralZoom(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/Integral_Zoom")
        {
            var raw = float.Parse(e.Data.Trim()[..^1]);
            var fov = CalcBlend(ConfigManager.Integral.Fovs, raw);
            State.Integral.FocalLength = 24 / MathF.Tan((fov / 2) * ((MathF.PI * 2) / 360)) / 2;
        }
    }

    public static void IntegralEnable(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/Integral_Enable")
        {
            State.IntegralEnabled = bool.Parse(e.Data);
        }
    }

    public static void IntegralMode(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/Integral_Mode")
        {
            var mode = int.Parse(e.Data);
            if (State.Integral.ExposureStartTime is not null)
            {
                var lastIndex = State.IntegralExposureParams.Count - 1;
                var lastItem = State.IntegralExposureParams[lastIndex];
                lastItem.ExposureTime = MathF.Min((float)(DateTime.Now - State.Integral.ExposureStartTime.Value).TotalSeconds, lastItem.ExposureTime);
                State.IntegralExposureParams[lastIndex] = lastItem;
            }
            State.Integral.ExposureStartTime = null;
            if (mode > 1)
            {
                var exposureParam = new IntegralExposureParam()
                {
                    ApertureValue = (State.Integral.FocalLength / 1000) / (2.0f * State.Integral.ApertureSize),
                    FocalLength = State.Integral.FocalLength,
                    ExposureBias = State.Integral.ExposureBias,
                    LensModel = ConfigManager.Integral.BokehShapeNames[State.Integral.BokehShape],
                    ExposureTime = float.PositiveInfinity,
                };
                switch (mode)
                {
                    /* Bulb */
                    case 2:
                    /*Open*/
                    case 3:
                        State.Integral.ExposureStartTime = DateTime.Now;
                        State.IntegralExposureParams.Add(exposureParam);
                        break;
                    /* Single */
                    case 4:
                        State.Integral.ExposureStartTime = null;
                        exposureParam.ExposureTime = 0;
                        State.IntegralExposureParams.Add(exposureParam);
                        break;
                    /* Clear */
                    case 5:
                        State.Integral.ExposureStartTime = null;
                        State.IntegralExposureParams.Clear();
                        break;
                    /* Clear And Open */
                    case 6:
                        State.IntegralExposureParams.Clear();
                        State.Integral.ExposureStartTime = DateTime.Now;
                        State.IntegralExposureParams.Add(exposureParam);
                        break;
                    /* SS */
                    case 7:
                        exposureParam.ExposureTime = State.Integral.ExposureTime;
                        State.IntegralExposureParams.Add(exposureParam);
                        break;
                }
            }
        }
    }

    public static void IntegralAperture(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/Integral_Aperture")
        {
            var raw = float.Parse(e.Data.Trim()[..^1]);
            var apature = CalcBlend(ConfigManager.Integral.Apatures, raw);
            State.Integral.ApertureSize = apature;
        }
    }

    public static void IntegralShutterSpeed(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/Integral_ShutterSpeed")
        {
            var raw = float.Parse(e.Data.Trim()[..^1]);
            var ss = CalcBlend(ConfigManager.Integral.ShutterSpeeds, raw);
            State.Integral.ExposureTime = ss;
        }
    }

    public static void IntegralExposure(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/Integral_Exposure")
        {
            var raw = float.Parse(e.Data.Trim()[..^1]);
            var ex = CalcBlend(ConfigManager.Integral.Exposures, raw);
            State.Integral.ExposureBias = MathF.Log2(ex);

        }
    }

    public static void IntegralBokehShape(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/Integral_BokehShape")
        {
            var raw = int.Parse(e.Data);
            State.Integral.BokehShape = raw;

        }
    }

    public static void ChangeAvater(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/change")
        {
            State.VL2Enabled = false;
            State.VirtualLens2.FocalLength = ConfigManager.VirtualLens2.FocalLengthDefault;
            State.VirtualLens2.ApertureValue = ConfigManager.VirtualLens2.ApertureDefault;

            State.IntegralEnabled = false;
            State.Integral.FocalLength = ConfigManager.Integral.FocalLengthDefault;
            State.Integral.ApertureSize = ConfigManager.Integral.ApertureDefault;
            State.Integral.ExposureTime = ConfigManager.Integral.ExposureTimeDefault;
            State.Integral.ExposureBias = ConfigManager.Integral.ExposureDefault;
            State.IntegralExposureParams.Clear();
        }
    }
}
