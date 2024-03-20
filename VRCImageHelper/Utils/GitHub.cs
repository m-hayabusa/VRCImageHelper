namespace VRCImageHelper.Utils;

using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

internal class GitHub
{
#pragma warning disable IDE1006 // 命名スタイル, JSONのキー名のままとする
    public struct Assets
    {
        public string browser_download_url { get; set; }
        public string name { get; set; }
    }
    public struct Release
    {
        public string url { get; set; }
        public string html_url { get; set; }
        public Assets[] assets { get; set; }
        public string tag_name { get; set; }
    }
#pragma warning restore IDE1006 // 命名スタイル

    public static async Task<Release?> GetLatestRelease()
    {
        try
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "VRCImageHelper");

            var releases = await client.GetAsync(@"https://api.github.com/repos/m-hayabusa/VRCImageHelper/releases");
            var body = await releases.Content.ReadAsStringAsync();

            if (body is null)
                return null;

            var result = JsonSerializer.Deserialize<Release[]>(body);
            if (result is null || result.Length == 0)
                return null;

            return result[0];
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return null;
        }
    }
}
