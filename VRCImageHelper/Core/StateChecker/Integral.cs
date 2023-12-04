namespace VRCImageHelper.Core.StateChecker;

using VRCImageHelper.Core;

internal class IntegralExposureState
{
    public IntegralExposureState()
    {
        FocalLength = ConfigManager.Integral.FocalLengthDefault;
        ApertureValue = (FocalLength / 1000) / (2.0f * ConfigManager.Integral.ApertureDefault);
        ExposureTime = 0;
        ExposureBias = ConfigManager.Integral.ExposureDefault;
        LensModel = ConfigManager.Integral.BokehShapeNames[ConfigManager.Integral.BokehShapeDefault];
    }
    public IntegralExposureState(IntegralExposureState integralExposureState)
    {
        FocalLength = integralExposureState.FocalLength;
        ApertureValue = integralExposureState.ApertureValue;
        ExposureTime = integralExposureState.ExposureTime;
        ExposureBias = integralExposureState.ExposureBias;
        LensModel = integralExposureState.LensModel;
    }
    public float FocalLength { get; set; }
    public float ApertureValue { get; set; }
    public float ExposureBias { get; set; }
    public string LensModel { get; set; }
    public float ExposureTime { get; set; }
}
internal class IntegralState
{
    public IntegralState()
    {
        FocalLength = ConfigManager.Integral.FocalLengthDefault;
        ApertureSize = ConfigManager.Integral.ApertureDefault;
        ExposureTime = ConfigManager.Integral.ExposureTimeDefault;
        ExposureBias = ConfigManager.Integral.ExposureDefault;
        BokehShape = ConfigManager.Integral.BokehShapeDefault;
        ExposureState = new List<IntegralExposureState>();
    }
    public IntegralState(IntegralState integralState)
    {
        FocalLength = integralState.FocalLength;
        ApertureSize = integralState.ApertureSize;
        ExposureTime = integralState.ExposureTime;
        ExposureBias = integralState.ExposureBias;
        BokehShape = integralState.BokehShape;
        ExposureState = new List<IntegralExposureState>(integralState.ExposureState);
        Enabled = integralState.Enabled;
    }
    public float FocalLength { get; set; }
    public float ApertureSize { get; set; }
    public float ExposureTime { get; set; }
    public float ExposureBias { get; set; }
    public int BokehShape { get; set; }
    public DateTime? ExposureStartTime { get; set; }
    public bool Enabled { get; set; }
    // Integralは複数回露光できるため、露光時のパラメーターを記録する
    public List<IntegralExposureState> ExposureState { get; set; }
}

internal static class Integral
{
    public static List<string> Publish(IntegralState state)
    {
        var args = new List<string>();
        if (state.Enabled)
        {
            args.Add("-:Make=suzufactory");
            args.Add("-:Model=Integral");
            if (state.ExposureState.Count > 0)
            {
                var lastItem = state.ExposureState.Last();
                args.Add("-:LensMake=suzufactory");
                args.Add("-:LensManufacturer=suzufactory");
                args.Add($"-:LensModel={lastItem.LensModel}");
                args.Add($"-:XMP-microsoft:LensModel={lastItem.LensModel}");
                args.Add($"-:FocalLength={lastItem.FocalLength}");
                if (lastItem.ApertureValue != 0)
                    args.Add($"-:FNumber={lastItem.ApertureValue}");
                var exposureTimes = state.ExposureState.Select((param) => param.ExposureTime);
                var exposureTime = exposureTimes.Sum();
                args.Add($"-:ExposureTime={exposureTime}");
                if (!float.IsInfinity(lastItem.ExposureBias))
                    args.Add($"-:ExposureCompensation={lastItem.ExposureBias}");

                // 多重露光の情報を記録
                var exposureCount = state.ExposureState.Count;
                if (exposureCount == 1)
                {
                    args.Add("-:CompositeImage=1");
                }
                else
                {
                    args.Add("-:CompositeImage=3");
                    args.Add($"-:CompositeImageCount=\"{exposureCount} {exposureCount}\"");
                    var minimumExposureTime = exposureTimes.Min();
                    var maximumExposureTime = exposureTimes.Max();
                    var list = new List<float>() {
                        exposureTime,
                        exposureTime,
                        exposureTime,
                        maximumExposureTime,
                        maximumExposureTime,
                        minimumExposureTime,
                        minimumExposureTime,
                        exposureCount,
                        exposureCount,
                    };
                    args.Add($"-:CompositeImageExposureTimes=\"{string.Join(" ", list.Concat(exposureTimes))}\"");
                }
            }
        }
        return args;
    }

    private static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    private static float CalcBlend(float[] values, float raw)
    {
        raw *= values.Length - 1;
        return Lerp(values[(int)raw], values[(int)MathF.Min(raw + 1, values.Length - 1)], raw - ((int)raw));
    }

    public static void Zoom(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/Integral_Zoom")
        {
            var raw = float.Parse(e.Data.Trim()[..^1]);
            var fov = CalcBlend(ConfigManager.Integral.Fovs, raw);
            State.Current.Integral.FocalLength = 24 / MathF.Tan((fov / 2) * ((MathF.PI * 2) / 360)) / 2;
        }
    }

    public static void Enable(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/Integral_Enable")
        {
            State.Current.Integral.Enabled = bool.Parse(e.Data);
        }
    }

    public static void Mode(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/Integral_Mode")
        {
            var mode = int.Parse(e.Data);
            if (State.Current.Integral.ExposureStartTime is not null)
            {
                var lastIndex = State.Current.Integral.ExposureState.Count - 1;
                var lastItem = State.Current.Integral.ExposureState[lastIndex];
                lastItem.ExposureTime = MathF.Min((float)(DateTime.Now - State.Current.Integral.ExposureStartTime.Value).TotalSeconds, lastItem.ExposureTime);
                State.Current.Integral.ExposureState[lastIndex] = lastItem;
            }
            State.Current.Integral.ExposureStartTime = null;
            if (mode > 1)
            {
                var exposureState = new IntegralExposureState()
                {
                    ApertureValue = (State.Current.Integral.FocalLength / 1000) / (2.0f * State.Current.Integral.ApertureSize),
                    FocalLength = State.Current.Integral.FocalLength,
                    ExposureBias = State.Current.Integral.ExposureBias,
                    LensModel = ConfigManager.Integral.BokehShapeNames[State.Current.Integral.BokehShape],
                    ExposureTime = float.PositiveInfinity,
                };
                switch (mode)
                {
                    /* Bulb */
                    case 2:
                    /*Open*/
                    case 3:
                        State.Current.Integral.ExposureStartTime = DateTime.Now;
                        State.Current.Integral.ExposureState.Add(exposureState);
                        break;
                    /* Single */
                    case 4:
                        State.Current.Integral.ExposureStartTime = null;
                        exposureState.ExposureTime = 0;
                        State.Current.Integral.ExposureState.Add(exposureState);
                        break;
                    /* Clear */
                    case 5:
                        State.Current.Integral.ExposureStartTime = null;
                        State.Current.Integral.ExposureState.Clear();
                        break;
                    /* Clear And Open */
                    case 6:
                        State.Current.Integral.ExposureState.Clear();
                        State.Current.Integral.ExposureStartTime = DateTime.Now;
                        State.Current.Integral.ExposureState.Add(exposureState);
                        break;
                    /* SS */
                    case 7:
                        exposureState.ExposureTime = State.Current.Integral.ExposureTime;
                        State.Current.Integral.ExposureState.Add(exposureState);
                        break;
                }
            }
        }
    }

    public static void Aperture(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/Integral_Aperture")
        {
            var raw = float.Parse(e.Data.Trim()[..^1]);
            var apature = CalcBlend(ConfigManager.Integral.Apatures, raw);
            State.Current.Integral.ApertureSize = apature;
        }
    }

    public static void ShutterSpeed(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/Integral_ShutterSpeed")
        {
            var raw = float.Parse(e.Data.Trim()[..^1]);
            var ss = CalcBlend(ConfigManager.Integral.ShutterSpeeds, raw);
            State.Current.Integral.ExposureTime = ss;
        }
    }

    public static void Exposure(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/Integral_Exposure")
        {
            var raw = float.Parse(e.Data.Trim()[..^1]);
            var ex = CalcBlend(ConfigManager.Integral.Exposures, raw);
            State.Current.Integral.ExposureBias = MathF.Log2(ex);
        }
    }

    public static void BokehShape(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/Integral_BokehShape")
        {
            var raw = int.Parse(e.Data);
            State.Current.Integral.BokehShape = raw;
        }
    }
}

