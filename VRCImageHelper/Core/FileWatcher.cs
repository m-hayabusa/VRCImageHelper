namespace VRCImageHelper.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

internal class FileWatcher : IDisposable
{
    // <撮影時刻のDateTime, フルパス>
    public static SortedDictionary<DateTime, string> s_queue = new();

    // TODO: VRChatのconfig.json見る
    private readonly string _targetDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "VRChat");
    private readonly FileSystemWatcher _fsWatcher;
    private readonly Timer _refreshTimer;

    public void Dispose()
    {
        _refreshTimer.Dispose();
        _fsWatcher.EnableRaisingEvents = false;
    }
    public FileWatcher(CancellationToken token)
    {
        // FileSystemWatcherを初期化
        _fsWatcher = new()
        {
            Path = _targetDirectory,
            Filter = "*.*", // 全てのファイルを監視（特定の拡張子は後でフィルタ）
            NotifyFilter = NotifyFilters.FileName
        };

        Debug.WriteLine(_targetDirectory);

        // イベントハンドラを追加
        _fsWatcher.Created += OnFileCreated;
        _fsWatcher.Renamed += OnFileCreated;
        _fsWatcher.Changed += OnFileCreated;

        // サブディレクトリも監視
        _fsWatcher.IncludeSubdirectories = true;

        // 監視を開始
        _fsWatcher.EnableRaisingEvents = true;

        // 何かわからないけど、つついておかないと FileSystemWatcher のイベントが発火しない
        _refreshTimer = new Timer(10000)
        {
            AutoReset = true
        };
        _refreshTimer.Elapsed += new ElapsedEventHandler((_, _) =>
        {
            _fsWatcher.EnableRaisingEvents = false;
            _fsWatcher.EnableRaisingEvents = true;
        });
        _refreshTimer.Start();

        Debug.WriteLine("監視を開始しました");
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        Debug.WriteLine("OnFile " + e.ChangeType + " " + e.FullPath);

        ProcessFile(e.FullPath);
    }

    private static void ProcessFile(string path)
    {
        if (Path.GetExtension(path).ToLower() != ".png")
        {
            return;
        }
        var dateString = Regex.Match(Path.GetFileName(path), @"\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2}.\d{3}");

        if (dateString.Success)
        {
            // 日時文字列を変換
            if (DateTime.TryParseExact(dateString.Value, "yyyy-MM-dd_HH-mm-ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDateTime))
            {
                Debug.WriteLine($"抽出された日時: {parsedDateTime} {path}");
                if ((parsedDateTime - LogReader.CurrentHead).TotalSeconds < 5)
                {
                    Debug.WriteLine("先端じゃないのでログが追いつくまでまつ");
                    s_queue.Add(parsedDateTime, path);
                }
                else
                {
                    Debug.WriteLine("先端なのですぐに処理してよい");
                    var state = State.Current.Clone();
                    state.CreationDate = parsedDateTime.ToString("yyy:MM:dd HH:mm:ss");
                    new Task(() => ImageProcess.Process(path, state)).Start();
                }
            }
            else
            {
                Debug.WriteLine("日時の変換に失敗しました。" + dateString);
            }
        }
        else
        {
            Debug.WriteLine("ファイル名から日時情報を抽出できませんでした。");
        }
    }
}
