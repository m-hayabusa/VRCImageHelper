namespace VRCImageHelper;

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

internal class State
{
    public State()
    {
        FocalLength = ConfigManager.VirtualLens2.FocalLengthDefault;
        ApertureValue = ConfigManager.VirtualLens2.ApertureDefault;
        VL2Enabled = false;
        CreationDate = "";
        Players = new List<string>();
        RoomInfo = new RoomInfo();
    }

    public State Clone()
    {
        var clone = (State)MemberwiseClone();
        clone.Players = new List<string>(Players);
        clone.RoomInfo = new RoomInfo(RoomInfo);
        return clone;
    }
    [JsonIgnore]
    public string CreationDate { get; set; }
    [JsonIgnore]
    public bool VL2Enabled { get; set; }
    [JsonIgnore]
    public float FocalLength { get; set; }
    [JsonIgnore]
    public float ApertureValue { get; set; }
    public List<string> Players { get; set; }
    [JsonPropertyName("roominfo")]
    public RoomInfo RoomInfo { get; set; }
}

internal class Info
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
            var raw = Single.Parse(e.Data.Trim()[..^1]);
            var min = ConfigManager.VirtualLens2.FocalLengthMin;
            var max = ConfigManager.VirtualLens2.FocalLengthMax;
            State.FocalLength = min * MathF.Exp(raw * MathF.Log(max / min));
        }
    }

    public static void VL2Enable(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/parameters/VirtualLens2_Enable")
        {
            State.VL2Enabled = Int32.Parse(e.Data) == 1;
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
                State.ApertureValue = float.PositiveInfinity;
            else
                State.ApertureValue = min * MathF.Exp(raw * MathF.Log(max / min));
        }
    }

    public static void ChangeAvater(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/change")
        {
            State.VL2Enabled = false;
            State.FocalLength = ConfigManager.VirtualLens2.FocalLengthDefault;
            State.ApertureValue = ConfigManager.VirtualLens2.ApertureDefault;
        }
    }
}
