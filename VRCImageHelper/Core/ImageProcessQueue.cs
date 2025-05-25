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

    private static DateTime s_lastEnqueuedTime = DateTime.MinValue;
    private static readonly object s_lockObject = new();

    static ImageProcessQueue()
    {
        s_timer = new Timer(10000);
        s_timer.Elapsed += (sender, e) => CheckQueue();
        s_timer.AutoReset = false;

        if (ConfigManager.ParallelCompressionProcesses > 0)
        {
            s_compressSemaphore = new SemaphoreSlimWrapper(ConfigManager.ParallelCompressionProcesses, ConfigManager.ParallelCompressionProcesses);
        }
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
                if (timestamp <= s_lastEnqueuedTime)
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
                    CheckQueue();
                }
            }
        }
    }

    public static void CheckQueue()
    {
        ResetTimer();

        var currentLogTime = LogReader.CurrentHead;

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
                            using (s_compressSemaphore?.Wait())
                            {
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
