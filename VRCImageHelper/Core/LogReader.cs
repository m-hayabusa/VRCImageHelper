namespace VRCImageHelper.Core;

using System.Timers;

public class NewLineEventArgs : EventArgs
{
    public NewLineEventArgs(string line) => Line = line;

    public string Line { get; }
}
public delegate void NewLineEventHandler(object sender, NewLineEventArgs e);

internal class LogReader : IDisposable
{
    private readonly Timer _refreshTimer;
    private readonly string _logDir;
    private readonly CancellationToken _cancellationToken;
    private readonly FileSystemWatcher _fsWatcher;
    private FileInfo? _logFile;
    private long _head;
    private bool _enabled;

    public event NewLineEventHandler? NewLine;

    public LogReader(CancellationToken token)
    {
        _cancellationToken = token;
        _logDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low\\VRChat\\VRChat\\";

        _logFile = FindLogFile();

        _fsWatcher = new FileSystemWatcher(_logDir)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName
        };

        _fsWatcher.Created += Watcher_Created;
        _fsWatcher.Changed += Watcher_Changed;

        // 何かわからないけど、つついておかないと FileSystemWatcher のイベントが発火しない
        _refreshTimer = new Timer(500)
        {
            AutoReset = true
        };
        _refreshTimer.Elapsed += new ElapsedEventHandler((_, _) =>
        {
            if (_logFile is not null && _logFile.Exists)
            {
                _logFile.Refresh();
            }
        });
    }
    public void Dispose()
    {
        _refreshTimer.Dispose();
        _fsWatcher.EnableRaisingEvents = false;
    }

    public void Enable()
    {
        if (_enabled)
        {
            return;
        }
        _enabled = true;

        _refreshTimer.Enabled = true;
        _fsWatcher.EnableRaisingEvents = true;
        SeeqLog();
    }

    private void Watcher_Created(object sender, FileSystemEventArgs e)
    {
        var newLogFile = FindLogFile();
        if (newLogFile != _logFile && newLogFile is not null)
        {
            _logFile = newLogFile;
            _head = 0;
            SeeqLog();
        }
    }

    private void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType == WatcherChangeTypes.Changed)
        {
            SeeqLog();
        }
    }

    private string _lastLine = "";
    private bool _seeqLogLock;
    private void SeeqLog()
    {
        if (_logFile is null || !_logFile.Exists || _seeqLogLock == true)
            return;

        _seeqLogLock = true;
        var logStream = new StreamReader(_logFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

        _ = logStream.BaseStream.Seek(_head, SeekOrigin.Current);

        var log = logStream.ReadToEnd().Split('\n');
        log[0] = _lastLine + log[0];
        for (var i = 0; i < log.Length - 1; i++)
        {
            if (_cancellationToken.IsCancellationRequested)
            { break; }

            var newline = log[i];
            if (newline.Length < 500 && !newline.StartsWith(" ") && newline.Trim() != "" && !newline.Contains("Error      -  ") && !newline.Contains("Warning    -  "))
            {
                var e = new NewLineEventArgs(newline);
                NewLine?.Invoke(this, e);
            }
        }
        _lastLine = log[^1];

        _head = logStream.BaseStream.Position;

        logStream.Close();

        _seeqLogLock = false;
    }

    private FileInfo? FindLogFile()
    {
        var logFile = Directory.EnumerateFiles(_logDir, "output_log_*.txt", SearchOption.TopDirectoryOnly)
            .ToList()
            .OrderByDescending(f => File.GetCreationTime(f));
        return logFile.Any() ? new FileInfo(logFile.First()) : null;
    }
}
