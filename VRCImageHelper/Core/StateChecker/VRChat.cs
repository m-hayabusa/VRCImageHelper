namespace VRCImageHelper.Core.StateChecker;

using System.Diagnostics;
using System.Text.RegularExpressions;

internal static class VRChat
{
    public static void WorldId(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, @".*\[Behaviour\] Joining (?<WorldName>wrld_.*?):(?<InstanceID>.*?)~(?<Options>.*)?");
        if (match.Success)
        {
            Debug.WriteLine($"Joining {match.Groups["WorldName"]}, {match.Groups["InstanceID"]}, {match.Groups["Options"]}");
            State.Current.RoomInfo.World_id = match.Groups["WorldName"].Value;
            State.Current.RoomInfo.Instance_id = match.Groups["InstanceID"].Value;

            if (match.Groups["Options"].Success)
            {
                var options = match.Groups["Options"].Value.Split("~").Select(param =>
                {
                    var match = Regex.Match(param, @"^(.*?)(?:\((.*?)\))?$");
                    return new[] { match.Groups[1].Value, match.Groups[2].Value };
                });

                State.Current.RoomInfo.Permission = "public";
                State.Current.RoomInfo.Organizer = "public";

                foreach (var option in options)
                {
                    var key = option[0];
                    var value = option[1];

                    if (key == "private" || key == "friends" || key == "hidden" || key == "group")
                    {
                        State.Current.RoomInfo.Permission = key;
                        State.Current.RoomInfo.Organizer = value;
                    }

                    if (key == "canRequestInvite")
                    {
                        State.Current.RoomInfo.Permission += "_plus";
                    }

                    if (key == "groupAccessType")
                    {
                        State.Current.RoomInfo.Permission += "_" + value;
                    }
                }
            }
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
