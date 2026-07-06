namespace VRCImageHelper.Core;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

internal struct QueueTask
{
    public QueueTask(string path)
    {
        this.path = path;
    }

    public void SetIsProcessing(bool isProcessing)
    {
        this.isProcessing = isProcessing;
    }

    public void SetState(State state)
    {
        this.state = state;
    }

    public string path;
    public bool isProcessing = false;
    public State? state = null;
}

/// <summary>
/// 画像処理のキューを管理するクラス
/// </summary>
internal static class ImageProcessQueue
{
    public static SemaphoreSlimWrapper? s_compressSemaphore;
    public static SortedDictionary<DateTime, LinkedList<QueueTask>> s_queue = new();
    private static readonly Timer s_timer;

    private static DateTime s_lastEnqueuedTime;
    private static readonly object s_lockObject = new();
    private static readonly string s_lastProcessedTimeFile = "last_processed_time.txt";


    // 初期化メソッドを追加
    public static void Initialize(bool scanAll = false)
    {
        s_lastEnqueuedTime = LoadLastProcessedTime(scanAll) + TimeSpan.FromMilliseconds(1);
    }

    static ImageProcessQueue()
    {
        s_timer = new Timer(1000);
        s_timer.Elapsed += (sender, e) => CheckQueue();
        s_timer.AutoReset = false;

        if (ConfigManager.ParallelCompressionProcesses > 0)
        {
            s_compressSemaphore = new SemaphoreSlimWrapper(ConfigManager.ParallelCompressionProcesses, ConfigManager.ParallelCompressionProcesses);
        }
        ResetTimer();
    }

    private static void SaveLastProcessedTime(DateTime time)
    {
        try
        {
            File.WriteAllText(s_lastProcessedTimeFile, time.ToString("O"));
        }
        catch (Exception)
        {
            // ファイル書き込みに失敗した場合は無視
        }
    }

    private static DateTime LoadLastProcessedTime(bool scanAll = false)
    {
        // scanAllがtrueの場合は常にMinValueを返す
        if (scanAll)
        {
            return DateTime.MinValue;
        }

        try
        {
            if (File.Exists(s_lastProcessedTimeFile))
            {
                var timeStr = File.ReadAllText(s_lastProcessedTimeFile);
                if (DateTime.TryParse(timeStr, out DateTime time))
                {
                    return time;
                }
            }
        }
        catch (Exception)
        {
            SaveLastProcessedTime(DateTime.MinValue);
        }
        return DateTime.MinValue;
    }

    private static void ResetTimer()
    {
        s_timer.Stop();
        s_timer.Start();
    }

    public static void Enqueue(string path, State? state = null)
    {
        if (ParseDate.TryParseFilePathToDateTime(path, out var timestamp))
        {
            lock (s_lockObject)
            {
                if (timestamp < s_lastEnqueuedTime)
                {
                    return;
                }

                if (!s_queue.ContainsKey(timestamp))
                {
                    s_queue[timestamp] = new LinkedList<QueueTask>();
                }

                if (!s_queue[timestamp].Any(item => item.path == path))
                {
                    s_queue[timestamp].AddLast(new QueueTask(path) { state = state });
                    s_lastEnqueuedTime = timestamp;
                }
            }

            ResetTimer();
        }
    }

    public static void CheckQueue()
    {
        ResetTimer();

        var currentLogTime = LogReader.CurrentHead;

        var now = DateTime.Now - TimeSpan.FromSeconds(5);

        // 5秒以上進んでいなければ、該当のファイルに関連するログの書き込みは存在しないとみなす
        if (currentLogTime < now)
        {
            currentLogTime = now;
        }

        lock (s_lockObject)
        {
            var keysToProcess = s_queue.Keys.Where(key => key < currentLogTime).ToList();

            foreach (var key in keysToProcess)
            {
                if (!s_queue.TryGetValue(key, out var taskList))
                    continue;

                var itemsToProcess = taskList.Where(item => !item.isProcessing).ToList();

                foreach (var item in itemsToProcess)
                {
                    var node = taskList.Find(item);
                    if (node != null)
                    {
                        var updatedItem = node.Value;
                        updatedItem.SetIsProcessing(true);
                        node.Value = updatedItem;

                        var state = item.state ?? State.Current.Clone();
                        state.CreationDate = key.ToString("yyyy:MM:dd HH:mm:ss");

                        Task.Run(() =>
                        {
                            Debug.WriteLine($"画像処理: 待機中: {item.path}");
                            using (s_compressSemaphore?.Wait())
                            {
                                Debug.WriteLine($"画像処理: 開始: {item.path}");
                                try
                                {
                                    ImageProcessor.ProcessImage(item.path, state);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"画像処理でエラーが発生しました: {item.path}, エラー: {ex.Message}");
                                }
                                finally
                                {
                                    lock (s_lockObject)
                                    {
                                        if (s_queue.TryGetValue(key, out var currentTaskList))
                                        {
                                            var nodeToRemove = currentTaskList.First;
                                            while (nodeToRemove != null)
                                            {
                                                var nextNode = nodeToRemove.Next;
                                                if (nodeToRemove.Value.path == item.path)
                                                {
                                                    currentTaskList.Remove(nodeToRemove);
                                                    break;
                                                }
                                                nodeToRemove = nextNode;
                                            }

                                            if (currentTaskList.Count == 0)
                                            {
                                                s_queue.Remove(key);
                                            }
                                        }

                                        if (!s_queue.Keys.Any(k => k < key))
                                        {
                                            SaveLastProcessedTime(key);
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
            }
        }
    }
}
