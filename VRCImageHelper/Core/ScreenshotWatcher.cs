namespace VRCImageHelper.Core;

using System.Text.RegularExpressions;

/// <summary>
/// VRChatのスクリーンショット撮影を監視するクラス
/// </summary>
internal static class ScreenshotWatcher
{
    private static readonly Regex s_screenshotRegex = new(@"[0-9.: ]* (?:Log|Debug) +? -  \[VRC Camera\] Took screenshot to: (?<Path>.*)");

    /// <summary>
    /// ログの新しい行を監視してスクリーンショット撮影を検出
    /// </summary>
    public static void OnNewLogLine(object sender, NewLineEventArgs e)
    {
        var match = s_screenshotRegex.Match(e.Line);
        if (match.Success)
        {
            var screenshotPath = match.Groups["Path"].ToString();
            var currentState = State.Current.Clone();
            
            // 処理キューに追加
            ImageProcessQueue.Enqueue(screenshotPath, currentState);
        }
    }
}
