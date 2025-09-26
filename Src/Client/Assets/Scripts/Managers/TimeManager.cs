using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoSingleton<TimerManager>
{
    #region Core Data Structures
    // 优先队列（最小堆），按触发时间排序
    private PriorityQueue<TimerTask> taskQueue = new PriorityQueue<TimerTask>();

    // 快速查找字典
    private Dictionary<string, TimerTask> taskDictionary = new Dictionary<string, TimerTask>();

    // 下一帧需要处理的任务缓存
    private List<TimerTask> tasksToProcess = new List<TimerTask>();
    #endregion

    #region Main Update Loop
    void Update()
    {
        ProcessTimers();
    }

    private void ProcessTimers()
    {
        float currentTime = Time.time;

        // 检查队列中是否有任务需要执行
        while (taskQueue.Count > 0 && taskQueue.Peek().triggerTime <= currentTime)
        {
            TimerTask task = taskQueue.Dequeue();

            if (task.isActive)
            {
                tasksToProcess.Add(task);
            }
        }

        // 处理所有到期的任务
        for (int i = 0; i < tasksToProcess.Count; i++)
        {
            TimerTask task = tasksToProcess[i];
            ProcessSingleTask(task, currentTime);
        }

        tasksToProcess.Clear();
    }

    private void ProcessSingleTask(TimerTask task, float currentTime)
    {
        try
        {
            // 执行回调
            task.callback?.Invoke(task.id);

            // 处理循环任务
            if (task.isLooping && task.isActive)
            {
                task.triggerTime = currentTime + task.duration;
                taskQueue.Enqueue(task);
            }
            else
            {
                // 单次任务，从字典中移除
                taskDictionary.Remove(task.id);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Timer task {task.id} callback error: {e.Message}");
            taskDictionary.Remove(task.id);
        }
    }
    #endregion

    #region Public API
    /// <summary>
    /// 添加计时任务
    /// </summary>
    /// <param name="id">任务唯一标识</param>
    /// <param name="duration">持续时间（秒）</param>
    /// <param name="callback">回调函数</param>
    /// <param name="isLooping">是否循环</param>
    public void AddTask(string id, float duration, Action<string> callback, bool isLooping = false)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("Task ID cannot be null or empty!");
            return;
        }

        if (taskDictionary.ContainsKey(id))
        {
            Debug.LogWarning($"Task with ID '{id}' already exists! Use UpdateTask to modify.");
            return;
        }

        TimerTask task = new TimerTask();
        task.Initialize(id, duration, callback, isLooping);

        taskDictionary[id] = task;
        taskQueue.Enqueue(task);
    }

    /// <summary>
    /// 添加计时任务（使用Action回调，不带参数）
    /// </summary>
    public void AddTask(string id, float duration, Action callback, bool isLooping = false)
    {
        AddTask(id, duration, (taskId) => callback(), isLooping);
    }

    /// <summary>
    /// 更新已存在的任务
    /// </summary>
    public void UpdateTask(string id, float newDuration, bool newIsLooping = false)
    {
        if (taskDictionary.TryGetValue(id, out TimerTask task))
        {
            // 先移除旧任务
            RemoveTask(id);

            // 添加新任务
            AddTask(id, newDuration, task.callback, newIsLooping);
        }
    }

    /// <summary>
    /// 移除任务
    /// </summary>
    public void RemoveTask(string id)
    {
        if (taskDictionary.TryGetValue(id, out TimerTask task))
        {
            taskDictionary.Remove(id);
            task.isActive = false; // 标记为无效，在队列中会被跳过
        }
    }

    /// <summary>
    /// 暂停任务
    /// </summary>
    public void PauseTask(string id)
    {
        if (taskDictionary.TryGetValue(id, out TimerTask task))
        {
            task.isActive = false;
        }
    }

    /// <summary>
    /// 恢复任务
    /// </summary>
    public void ResumeTask(string id)
    {
        if (taskDictionary.TryGetValue(id, out TimerTask task))
        {
            task.isActive = true;
        }
    }

    /// <summary>
    /// 检查任务是否存在
    /// </summary>
    public bool HasTask(string id)
    {
        return taskDictionary.ContainsKey(id);
    }

    /// <summary>
    /// 获取任务剩余时间
    /// </summary>
    public float GetRemainingTime(string id)
    {
        if (taskDictionary.TryGetValue(id, out TimerTask task))
        {
            return Mathf.Max(0, task.triggerTime - Time.time);
        }
        return -1f;
    }

    /// <summary>
    /// 清除所有任务
    /// </summary>
    public void ClearAllTasks()
    {
        taskQueue.Clear();
        taskDictionary.Clear();
    }

    /// <summary>
    /// 获取活跃任务数量
    /// </summary>
    public int GetActiveTaskCount()
    {
        return taskDictionary.Count;
    }
    #endregion

    #region Static Convenience Methods
    /// <summary>
    /// 静态方法：添加延迟执行的任务
    /// </summary>
    public static void Delay(float delay, Action callback)
    {
        string id = "delay_" + Guid.NewGuid().ToString();
        Instance.AddTask(id, delay, callback);
    }

    /// <summary>
    /// 静态方法：添加循环执行的任务
    /// </summary>
    public static void Repeat(string id, float interval, Action callback)
    {
        Instance.AddTask(id, interval, callback, true);
    }

    /// <summary>
    /// 静态方法：添加一次性任务
    /// </summary>
    public static void Once(string id, float delay, Action callback)
    {
        Instance.AddTask(id, delay, callback);
    }
    #endregion
}

#region Timer Task Class
public class TimerTask : IComparable<TimerTask>
{
    public string id;
    public float duration;
    public float triggerTime;
    public Action<string> callback;
    public bool isLooping;
    public bool isActive;

    public void Initialize(string id, float duration, Action<string> callback, bool isLooping = false)
    {
        this.id = id;
        this.duration = duration;
        this.callback = callback;
        this.isLooping = isLooping;
        this.isActive = true;
        this.triggerTime = Time.time + duration;
    }

    // 实现IComparable用于优先队列排序
    public int CompareTo(TimerTask other)
    {
        return triggerTime.CompareTo(other.triggerTime);
    }
}
#endregion

#region Priority Queue Implementation
public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> data;

    public int Count { get { return data.Count; } }

    public PriorityQueue()
    {
        this.data = new List<T>();
    }

    public void Enqueue(T item)
    {
        data.Add(item);
        int childIndex = data.Count - 1;

        while (childIndex > 0)
        {
            int parentIndex = (childIndex - 1) / 2;
            if (data[childIndex].CompareTo(data[parentIndex]) >= 0)
                break;

            T tmp = data[childIndex];
            data[childIndex] = data[parentIndex];
            data[parentIndex] = tmp;
            childIndex = parentIndex;
        }
    }

    public T Dequeue()
    {
        int lastIndex = data.Count - 1;
        T frontItem = data[0];
        data[0] = data[lastIndex];
        data.RemoveAt(lastIndex);

        lastIndex--;
        int parentIndex = 0;

        while (true)
        {
            int childIndex = parentIndex * 2 + 1;
            if (childIndex > lastIndex) break;

            int rightChild = childIndex + 1;
            if (rightChild <= lastIndex && data[rightChild].CompareTo(data[childIndex]) < 0)
                childIndex = rightChild;

            if (data[parentIndex].CompareTo(data[childIndex]) <= 0)
                break;

            T tmp = data[parentIndex];
            data[parentIndex] = data[childIndex];
            data[childIndex] = tmp;
            parentIndex = childIndex;
        }

        return frontItem;
    }

    public T Peek()
    {
        return data[0];
    }

    public void Clear()
    {
        data.Clear();
    }
}
#endregion