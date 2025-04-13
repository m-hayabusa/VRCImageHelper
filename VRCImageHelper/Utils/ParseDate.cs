
using System.Globalization;
using System.Text.RegularExpressions;

internal static class ParseDate
{
    public static bool TryParseFilePathToDateTime(string path, out DateTime parsedDateTime)
    {
        var dateString = Regex.Match(Path.GetFileName(path), @"\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2}.\d{3}");

        if (dateString.Success)
        {
            // 日時文字列を変換
            return DateTime.TryParseExact(dateString.Value, "yyyy-MM-dd_HH-mm-ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);
        }
        parsedDateTime = new(0);
        return false;
    }

    public static bool TryParseDirectoryPathToDateTime(string path, out DateTime parsedDateTime)
    {
        return DateTime.TryParseExact(Path.GetDirectoryName(path), "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);
    }
}
