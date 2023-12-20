namespace VRCImageHelper.Core;

using System.Text.Json.Serialization;
using StateChecker;

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
    public static State Current { get; set; } = new State();

    public State()
    {
        VirtualLens2 = new VirtualLens2State();
        CreationDate = "";
        Players = new List<string>();
        RoomInfo = new RoomInfo();
        Integral = new IntegralState();
    }

    public State Clone()
    {
        var clone = (State)MemberwiseClone();
        clone.Players = new List<string>(Players);
        clone.RoomInfo = new RoomInfo(RoomInfo);
        clone.VirtualLens2 = new VirtualLens2State(VirtualLens2);
        clone.Integral = new IntegralState(Integral);
        return clone;
    }
    [JsonIgnore]
    public string CreationDate { get; set; }
    [JsonIgnore]
    public VirtualLens2State VirtualLens2 { get; set; }
    [JsonIgnore]
    public IntegralState Integral { get; set; }
    public List<string> Players { get; set; }
    [JsonPropertyName("roominfo")]
    public RoomInfo RoomInfo { get; set; }
}
