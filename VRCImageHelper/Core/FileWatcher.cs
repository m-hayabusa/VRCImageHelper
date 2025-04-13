namespace VRCImageHelper.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public FileWatcher(DateTime start, CancellationToken token)
    {
        // start = 最初に読んだログの時刻より新しいすべてのスクショをキューに積む
        AddAllFileToQueueAfterDate(start);

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

        if (ParseDate.TryParseFilePathToDateTime(path, out var parsedDateTime))
        {
            Debug.WriteLine($"抽出された日時: {parsedDateTime} {path}");
            if ((parsedDateTime - LogReader.CurrentHead).TotalSeconds < 5)
            {
                if (!s_queue.Any(pair => pair.Key == parsedDateTime && pair.Value == path))
                {
                    Debug.WriteLine("FileWatcher: キューに積む " + path);
                    s_queue.Add(parsedDateTime, path);
                }
                else
                {
                    Debug.WriteLine("FileWatcher: すでにある " + path);
                }
            }
        }
        else
        {
            Debug.WriteLine("ファイル名から日時情報を抽出できませんでした。");
        }
    }

    private void AddAllFileToQueueAfterDate(DateTime threshold)
    {
        var files = Directory.EnumerateDirectories(_targetDirectory)
            .Select(dir => new { dir, sucess = ParseDate.TryParseDirectoryPathToDateTime(dir, out var date), date })
            .Where(x => x.sucess && (x.date >= threshold || (x.date.Year == threshold.Year && x.date.Month == threshold.Month))) // dateには月始めの日が入るので、「月が同じか、時刻が先のとき」になる
            .SelectMany(x => Directory.EnumerateFiles(x.dir, "*.png"))
            .Select(file => new { file, success = ParseDate.TryParseFilePathToDateTime(file, out var date), date })
            .Where(x => x.success && x.date >= threshold);

        foreach (var entry in files)
        {
            if (!s_queue.Any(pair => pair.Key == entry.date && pair.Value == entry.file))
            {
                s_queue.Add(entry.date, entry.file);
                Debug.WriteLine("FileWatcher: 初期化・キューに積む " + entry.file);
            }
        }
    }
}
