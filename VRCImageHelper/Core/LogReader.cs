namespace VRCImageHelper.Core;

using System.Globalization;
using System.Text.RegularExpressions;
using System.Timers;

public class NewLineEventArgs : EventArgs
{
    public NewLineEventArgs(string line) => Line = line;

    public string Line { get; }
}
public delegate void NewLineEventHandler(object sender, NewLineEventArgs e);
public class ScanAllProgressEventArgs : EventArgs
{
    public ScanAllProgressEventArgs(int processing, int total)
    {
        Processing = processing;
        Total = total;
    }

    public int Processing { get; }
    public int Total { get; }
}
public delegate void ScanAllProgressEventHandler(object sender, ScanAllProgressEventArgs e);

internal class LogReader : IDisposable
{
    private readonly Timer _refreshTimer;
    private static readonly string s_logDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low\\VRChat\\VRChat\\";
    private readonly CancellationToken _cancellationToken;
    private readonly FileSystemWatcher _fsWatcher;
    private FileInfo? _logFile;
    private long _head;
    private bool _enabled;

    public event NewLineEventHandler? NewLine;
    public event ScanAllProgressEventHandler? ScanAllProgress;

    public LogReader(CancellationToken token)
    {
        _cancellationToken = token;
        _fsWatcher = new FileSystemWatcher(s_logDir)
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

    public void Enable(bool scanAll)
    {
        if (_enabled)
        {
            return;
        }
        _enabled = true;

        var logFile = FindLogFile(null, scanAll);
        var progress = 0;
        var total = scanAll ? Directory.EnumerateFiles(s_logDir, "output_log_*.txt", SearchOption.TopDirectoryOnly).Count() : 1;
        do
        {
            ScanAllProgress?.Invoke(this, new ScanAllProgressEventArgs(progress, total));
            if (logFile is null) break;
            _logFile = logFile;
            _head = 0;
            SeeqLog();
            logFile = FindLogFile(logFile);
            progress++;
        } while (scanAll);
        ScanAllProgress?.Invoke(this, new ScanAllProgressEventArgs(0, 0));

        _refreshTimer.Enabled = true;
        _fsWatcher.EnableRaisingEvents = true;
    }

    private void Watcher_Created(object sender, FileSystemEventArgs e)
    {
        var newLogFile = FindLogFile();
        if (newLogFile != _logFile && newLogFile is not null)
        {
            _logFile = newLogFile;
            _head = 0;
            SeeqLog();

            var logChk = new Timer(10000);
            logChk.Elapsed += (s, e) =>
            {
                if (newLogFile.Exists && newLogFile.CreationTime == newLogFile.LastWriteTime)
                {
                    UI.SendNotify.Send(Properties.Resources.NotifyErrorLogFileNotBeWritten, false);
                }
                logChk.Dispose();
            };
            logChk.Enabled = true;
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
        while (!logStream.EndOfStream)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var newline = "";
            if (_lastLine != "" && logStream.Peek() == '\r')
            {
                logStream.Read();
                if (logStream.Peek() == '\n')
                    logStream.Read();
            }
            else
            {
                try
                {
                    newline = logStream.ReadLine();
                }
                catch (OutOfMemoryException) // 長すぎる行は読み飛ばす
                {
                    var lineEnd = false;
                    while (!lineEnd)
                    {
                        try
                        {
                            logStream.ReadLine();
                            lineEnd = true;
                        }
                        catch (OutOfMemoryException)
                        {
                            lineEnd = false;
                        }
                    }
                    continue;
                }
                if (newline is null)
                {
                    break;
                }
                if (logStream.EndOfStream)
                {
                    logStream.BaseStream.Seek(-1, SeekOrigin.End);
                    if (logStream.Read() != '\n')
                    {
                        _lastLine = newline;
                        break;
                    }
                }
            }

            newline = _lastLine + newline;
            _lastLine = "";

            if (newline.Length < 500 && !newline.StartsWith(" ") && newline.Trim() != "" && !newline.Contains("Error      -  ") && !newline.Contains("Warning    -  "))
            {
                var e = new NewLineEventArgs(newline);
                NewLine?.Invoke(this, e);
            }
        }

        _head = logStream.BaseStream.Position;

        logStream.Close();

        _seeqLogLock = false;
    }

    public static FileInfo? FindLogFile(FileInfo? prev = null, bool old = false)
    {
        var logFiles = Directory.EnumerateFiles(s_logDir, "output_log_*.txt", SearchOption.TopDirectoryOnly)
                    .ToList()
                    .OrderBy(f => File.GetCreationTime(f));

        if (!logFiles.Any())
            return null;

        if (prev is null)
        {
            if (old)
                return new FileInfo(logFiles.First());
            else
                return new FileInfo(logFiles.Last());
        }

        if (!File.Exists(prev.FullName))
            return new FileInfo(logFiles.Last());

        var logFile = logFiles
            .SkipWhile(f => File.GetCreationTime(f) <= File.GetCreationTime(prev.FullName))
            .FirstOrDefault();

        if (!string.IsNullOrEmpty(logFile))
            return new FileInfo(logFile);

        return null;
    }

    public static DateTime CurrentHead { get; private set; } = new(0);

    public static void UpdateCurrentHead(object sender, NewLineEventArgs e)
    {
        var match = Regex.Match(e.Line, @"$\d{4}.\d{2}.\d{2} \d{2}:\d{2}:\d{2}");

        if (DateTime.TryParseExact(match.Value, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var currentLogTime))
        {
            CurrentHead = currentLogTime;
        }
    }
}
