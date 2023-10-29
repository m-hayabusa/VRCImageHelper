using System.Diagnostics;
using System.Timers;

namespace VRCImageHelper
{
    public class NewLineEventArgs : EventArgs
    {
        public NewLineEventArgs(string line)
        {
            Line = line;
        }

        public string Line { get; }
    }
    public delegate void NewLineEventHandler(object sender, NewLineEventArgs e);

    internal class LogReader
    {
        private StreamReader? logStream;
        private readonly System.Timers.Timer refreshTimer;
        private FileInfo logFile;
        private static FileSystemWatcher? watcher;
        private readonly string logDir;
        private long head = 0;

        private bool _enabled = false;
        public bool Enable
        {
            set
            {
                if (value == _enabled) return;
                if (watcher == null) return;

                if (value)
                {
                    refreshTimer.Enabled = true;
                    watcher.EnableRaisingEvents = true;
                    SeeqLog();
                }
                else
                {
                    refreshTimer.Enabled = false;
                    watcher.EnableRaisingEvents = false;
                }

                _enabled = value;
            }
            get
            {
                return _enabled;
            }
        }

        public event NewLineEventHandler? NewLine;
        private readonly CancellationToken cancellationToken;

        public LogReader(CancellationToken token)
        {
            cancellationToken = token;
            logDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low\\VRChat\\VRChat\\";

            logFile = FindLogFile();

            watcher = new FileSystemWatcher(logDir)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName
            };

            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Changed;

            // 何かわからないけど、つついておかないと FileSystemWatcher のイベントが発火しない
            refreshTimer = new System.Timers.Timer(500)
            {
                AutoReset = true
            };
            refreshTimer.Elapsed += new ElapsedEventHandler((object? sender, ElapsedEventArgs e) => { logFile.Refresh(); });
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            var newLogFile = FindLogFile();
            Debug.WriteLine("Watcher_Created: " + logFile + " " + newLogFile);
            if (newLogFile != logFile)
            {
                logFile = newLogFile;
                head = 0;
                SeeqLog();
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine("Watcher_Changed");
            if (e.ChangeType == WatcherChangeTypes.Changed)
                SeeqLog();
        }

        private void SeeqLog()
        {
            logStream = new StreamReader(logFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            logStream.BaseStream.Seek(head, SeekOrigin.Current);

            string? newline;

            do
            {
                if (cancellationToken.IsCancellationRequested) { break; }

                newline = logStream.ReadLine();
                if (newline != null && newline != "")
                {
                    var e = new NewLineEventArgs(newline);
                    NewLine?.Invoke(this, e);
                }
            }
            while (newline != null);

            head = logStream.BaseStream.Position;

            logStream.Close();
        }

        private FileInfo FindLogFile()
        {
            var logFile = Directory.EnumerateFiles(logDir, "output_log_*.txt", SearchOption.TopDirectoryOnly)
                .ToList()
                .OrderBy(f => File.GetCreationTime(f))
                .Last();
            return new FileInfo(logFile);
        }

    }
}
