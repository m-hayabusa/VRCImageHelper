namespace VRCImageHelper.Core.StateChecker;

using System.Diagnostics;
using System.Text.RegularExpressions;

internal static class VRChat
{
    public static void WorldId(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, ".*\\[Behaviour\\] Joining (wrld_.*?):(?:.*?(private|friends|hidden|group)\\((.*?)\\))?(~canRequestInvite|~groupAccessType\\(plus\\))?");
        if (match.Success)
        {
            Debug.WriteLine($"Joining {match.Groups[1]}, {match.Groups[2]}, {match.Groups[3]}, {match.Groups[4]}");
            State.Current.RoomInfo.World_id = match.Groups[1].Value;
            State.Current.RoomInfo.Permission = match.Groups[2].Value + (match.Groups[4].Success ? "+" : "");
            State.Current.RoomInfo.Organizer = match.Groups[3].Value;
            State.Current.Players.Clear();
        }
    }

    public static void JoinRoom(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, "\\[Behaviour\\] Joining or Creating Room: (.*)");
        if (match.Success)
        {
            Debug.WriteLine($"Joining {match.Groups[1]}");
            State.Current.RoomInfo.World_name = match.Groups[1].Value;
        }
    }

    public static void PlayerJoin(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, "\\[Behaviour\\] OnPlayerJoined (.*?) \\((usr_.*)\\)");
        if (match.Success)
        {
            Debug.WriteLine($"Join {match.Groups[1]}, {match.Groups[2]}");
            State.Current.Players.Add(match.Groups[1].Value);
        }
    }

    public static void PlayerLeft(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, "\\[Behaviour\\] OnPlayerLeft (.*) \\((usr_.*)\\)");
        if (match.Success)
        {
            Debug.WriteLine($"Left {match.Groups[1]}, {match.Groups[2]}");
            State.Current.Players.Remove(match.Groups[1].Value);
        }
    }

    public static void Quit(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, "VRCApplication: OnApplicationQuit");
        if (match.Success)
        {
            Debug.WriteLine("Quit");
            State.Current = new State();
        }
    }

    public static void ChangeAvater(object sender, OscEventArgs e)
    {
        if (e.Path == "/avatar/change")
        {
            State.Current.VirtualLens2 = new VirtualLens2State();
            State.Current.Integral = new IntegralState();
        }
    }
}
