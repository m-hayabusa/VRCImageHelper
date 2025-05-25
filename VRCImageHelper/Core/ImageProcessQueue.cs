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

    static ImageProcessQueue()
    {
        s_timer = new Timer(10000);
        s_timer.Elapsed += (sender, e) => CheckQueue();
        s_timer.AutoReset = false; // 一度タイムアウトしたら再実行を防ぐ

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
            Debug.WriteLine($"抽出された日時: {timestamp} {path}");
            if (!s_queue.ContainsKey(timestamp))
            {
                s_queue[timestamp] = new LinkedList<QueueTask>();
            }

            if (!s_queue[timestamp].Any(item => item.path == path))
            {
                s_queue[timestamp].AddLast(new QueueTask(path) { state = state });
                Debug.WriteLine($"Task '{path}' added at {timestamp}.");
                CheckQueue();
            }
            else
            {
                Debug.WriteLine($"Task '{path}' already exists at {timestamp}, not adding.");
            }
        }
        else
        {
            Debug.WriteLine("ファイル名から日時情報を抽出できませんでした。");
        }
    }

    public static void CheckQueue()
    {
        ResetTimer();

        var currentLogTime = LogReader.CurrentHead;

        foreach (var list in s_queue.Where(file => file.Key < currentLogTime))
        {
            Debug.WriteLine("CheckQueue foreach#LIST " + list.Key + " " + list.Value.Count);

            foreach (var item in list.Value)
            {
                Debug.WriteLine("  CheckQueue foreach#ITEM " + item.path);

                if (item.isProcessing)
                    break;
                item.SetIsProcessing(true);

                var state = item.state ?? State.Current.Clone();
                state.CreationDate = list.Key.ToString("yyyy:MM:dd HH:mm:ss");

                Debug.WriteLine("キューから処理" + item);
                new Task(() =>
                {
                    using (s_compressSemaphore?.Wait())
                    {
                        Debug.WriteLine("キューから処理: 実行中" + item);
                        ImageProcessor.ProcessImage(item.path, state);
                        list.Value.Remove(item);
                        if (list.Value.Count == 0)
                        {
                            s_queue.Remove(list.Key);
                        }
                    }
                }).Start();
            }
        }
    }
}
