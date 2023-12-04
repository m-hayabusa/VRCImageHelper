namespace VRCImageHelper.Core.StateChecker;

internal class VirtualLens2State
{
    public VirtualLens2State()
    {
        FocalLength = ConfigManager.VirtualLens2.FocalLengthDefault;
        ApertureValue = ConfigManager.VirtualLens2.ApertureDefault;
    }
    public VirtualLens2State(VirtualLens2State virtualLens2State)
    {
        FocalLength = virtualLens2State.FocalLength;
        ApertureValue = virtualLens2State.ApertureValue;
        Enabled = virtualLens2State.Enabled;
    }
    public float FocalLength { get; set; }
    public float ApertureValue { get; set; }
    public bool Enabled { get; set; }
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
        }
        return args;
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
            var min = ConfigManager.VirtualLens2.FocalLengthMin;
            var max = ConfigManager.VirtualLens2.FocalLengthMax;
            State.Current.VirtualLens2.FocalLength = min * MathF.Exp(raw * MathF.Log(max / min));
        }
    }

    public static void Aperture(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/VirtualLens2_Aperture")
        {
            var raw = float.Parse(e.Data.Trim()[..^1]);
            var min = ConfigManager.VirtualLens2.ApertureMin;
            var max = ConfigManager.VirtualLens2.ApertureMax;
            if (raw == 0)
                State.Current.VirtualLens2.ApertureValue = float.PositiveInfinity;
            else
                State.Current.VirtualLens2.ApertureValue = min * MathF.Exp(raw * MathF.Log(max / min));
        }
    }
}
