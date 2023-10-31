namespace VRCImageHelper;

using System.Diagnostics;
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
    private static FileSystemWatcher? s_watcher;
    private FileInfo _logFile;
    private long _head;
    private bool _enabled;

    public event NewLineEventHandler? NewLine;

    public LogReader(CancellationToken token)
    {
        _cancellationToken = token;
        _logDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low\\VRChat\\VRChat\\";

        _logFile = FindLogFile();

        s_watcher = new FileSystemWatcher(_logDir)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName
        };

        s_watcher.Created += Watcher_Created;
        s_watcher.Changed += Watcher_Changed;

        // 何かわからないけど、つついておかないと FileSystemWatcher のイベントが発火しない
        _refreshTimer = new Timer(500)
        {
            AutoReset = true
        };
        _refreshTimer.Elapsed += new ElapsedEventHandler((_, _) => _logFile.Refresh());
    }

    public bool Enable
    {
        set
        {
            if (value == _enabled)
            {
                return;
            }

            if (s_watcher is not null)
            {
                if (value)
                {
                    _refreshTimer.Enabled = true;
                    s_watcher.EnableRaisingEvents = true;
                    SeeqLog();
                }
                else
                {
                    _refreshTimer.Enabled = false;
                    s_watcher.EnableRaisingEvents = false;
                }

                _enabled = value;
            }
        }
        get => _enabled;
    }

    private void Watcher_Created(object sender, FileSystemEventArgs e)
    {
        var newLogFile = FindLogFile();
        Debug.WriteLine("Watcher_Created: " + _logFile + " " + newLogFile);
        if (newLogFile != _logFile)
        {
            _logFile = newLogFile;
            _head = 0;
            SeeqLog();
        }
    }

    private void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        Debug.WriteLine("Watcher_Changed");
        if (e.ChangeType == WatcherChangeTypes.Changed)
        {
            SeeqLog();
        }
    }

    private string _lastLine = "";
    private void SeeqLog()
    {
        var logStream = new StreamReader(_logFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

        _ = logStream.BaseStream.Seek(_head, SeekOrigin.Current);

        var log = logStream.ReadToEnd().Split('\n');
        log[0] += _lastLine;
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
    }

    private FileInfo FindLogFile()
    {
        var logFile = Directory.EnumerateFiles(_logDir, "output_log_*.txt", SearchOption.TopDirectoryOnly)
            .ToList()
            .OrderBy(f => File.GetCreationTime(f))
            .Last();
        return new FileInfo(logFile);
    }

    public void Dispose() => _refreshTimer.Dispose();
}
