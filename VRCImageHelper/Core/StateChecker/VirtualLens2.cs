namespace VRCImageHelper.Core.StateChecker;

internal class VirtualLens2State
{
    public VirtualLens2State()
    {
        FocalLength = ConfigManager.VirtualLens2.FocalLengthDefault;
        ApertureValue = ConfigManager.VirtualLens2.ApertureDefault;
        ExposureBias = ConfigManager.VirtualLens2.ExposureDefault;
        FocalLengthMin = ConfigManager.VirtualLens2.FocalLengthMin;
        FocalLengthMax = ConfigManager.VirtualLens2.FocalLengthMax;
        ApertureMin = ConfigManager.VirtualLens2.ApertureMin;
        ApertureMax = ConfigManager.VirtualLens2.ApertureMax;
        ExposureRange = ConfigManager.VirtualLens2.ExposureRange;
    }
    public VirtualLens2State(VirtualLens2State virtualLens2State)
    {
        FocalLength = virtualLens2State.FocalLength;
        ApertureValue = virtualLens2State.ApertureValue;
        ExposureBias = virtualLens2State.ExposureBias;
        Enabled = virtualLens2State.Enabled;
    }
    public float FocalLength { get; set; }
    public float ApertureValue { get; set; }
    public float ExposureBias { get; set; }
    public bool Enabled { get; set; }
    public float FocalLengthMin { get; set; }
    public float FocalLengthMax { get; set; }
    public float ApertureMin { get; set; }
    public float ApertureMax { get; set; }
    public float ExposureRange { get; set; }
}

internal static class VirtualLens2
{
    public static List<string> Publish(VirtualLens2State state)
    {
        var args = new List<string>();

        if (state.Enabled)
        {
            args.Add("-:Make=logilabo");
            args.Add("-:Model=VirtualLens2");
            args.Add($"-:FocalLength={state.FocalLength}");
            if (!float.IsInfinity(state.ApertureValue))
                args.Add($"-:FNumber={state.ApertureValue}");
            if (!float.IsInfinity(state.ExposureBias))
                args.Add($"-:ExposureCompensation={state.ExposureBias}");
        }
        return args;
    }
    public static void Initialize(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/VirtualLens2_Zoom_Min")
        {
            State.Current.VirtualLens2.FocalLengthMin = float.Parse(e.Data.Trim()[..^1]);
        }
        else if (e.Path == "/avatar/parameters/VirtualLens2_Zoom_Max")
        {
            State.Current.VirtualLens2.FocalLengthMax = float.Parse(e.Data.Trim()[..^1]);
        }
        else if (e.Path == "/avatar/parameters/VirtualLens2_Aperture_Min")
        {
            State.Current.VirtualLens2.ApertureMin = float.Parse(e.Data.Trim()[..^1]);
        }
        else if (e.Path == "/avatar/parameters/VirtualLens2_Aperture_Max")
        {
            State.Current.VirtualLens2.ApertureMax = float.Parse(e.Data.Trim()[..^1]);
        }
        else if (e.Path == "/avatar/parameters/VirtualLens2_Exposure_Range")
        {
            State.Current.VirtualLens2.ExposureRange = float.Parse(e.Data.Trim()[..^1]);
        }
    }
    public static void Enable(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/VirtualLens2_Enable")
        {
            State.Current.VirtualLens2.Enabled = int.Parse(e.Data) == 1;
        }
    }

    public static void Zoom(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/VirtualLens2_Zoom")
        {
            var raw = float.Parse(e.Data.Trim()[..^1]);
            var min = State.Current.VirtualLens2.FocalLengthMin;
            var max = State.Current.VirtualLens2.FocalLengthMax;
            State.Current.VirtualLens2.FocalLength = min * MathF.Exp(raw * MathF.Log(max / min));
        }
    }

    public static void Aperture(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/VirtualLens2_Aperture")
        {
            var raw = float.Parse(e.Data.Trim()[..^1]);
            var min = State.Current.VirtualLens2.ApertureMin;
            var max = State.Current.VirtualLens2.ApertureMax;
            if (raw == 0)
                State.Current.VirtualLens2.ApertureValue = float.PositiveInfinity;
            else
                State.Current.VirtualLens2.ApertureValue = min * MathF.Exp(raw * MathF.Log(max / min));
        }
    }
    public static void Exposure(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/VirtualLens2_Exposure")
        {
            var raw = float.Parse(e.Data.Trim()[..^1]);
            State.Current.VirtualLens2.ExposureBias = (2 * raw - 1) * State.Current.VirtualLens2.ExposureRange;
        }
    }
}
