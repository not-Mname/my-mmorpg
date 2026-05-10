using Utilities.Time;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class TimerManager : MonoSingleton<TimerManager>
{
    #region Core Data Structures
    // 优先队列（最小堆），按触发时间排序
    private PriorityQueue<TimerTask> taskQueue = new PriorityQueue<TimerTask>();

    // 快速查找字典
    private Dictionary<string, TimerTask> taskDictionary = new Dictionary<string, TimerTask>();

    // 下一帧需要处理的任务缓存
    private List<TimerTask> tasksToProcess = new List<TimerTask>(5);
    #endregion

    #region Main Update Loop
    void Update()
    {
        ProcessTimers();
    }

    private void ProcessTimers()
    {
        float currentTime = UnityEngine.Time.time;
        //int processedTasks = this.taskDictionary.Count / 10 ;
        // 检查队列中是否有任务需要执行
        while (taskQueue.Count > 0)
        {
            TimerTask task = taskQueue.Peek();
            if (task.TriggerTime <= currentTime)
            {
                task = taskQueue.Dequeue();

                if (task.IsActive)// 如果任务仍然活跃，则加入到待处理列表中
                {
                    tasksToProcess.Add(task);
                }
                else
                {
                    taskDictionary.Remove(task.IdCallback);
                    task.Dispose();
                }

                if (tasksToProcess.Count > 5)// 如果一次处理的任务过多，为了避免卡顿，可以选择暂停或减少处理的数量
                {
                    break;
                }
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
            task.FireCallback();
            taskDictionary.Remove(task.IdCallback);
            task.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogError($"Timer task {task.IdCallback} callback error: {e.Message}");
            taskDictionary.Remove(task.IdCallback);
        }
    }
    #endregion

    #region 任务管理方法
    /// <summary>
    /// 移除任务
    /// </summary>
    public void RemoveTask(string id)
    {
        if (taskDictionary.TryGetValue(id, out TimerTask task))
        {
            taskDictionary.Remove(id);
            task.IsActive = false; // 标记为无效，在队列中会被跳过
        }
    }

    /// <summary>
    /// 暂停任务
    /// </summary>
    public void PauseTask(string id)
    {
        if (taskDictionary.TryGetValue(id, out TimerTask task))
        {
            task.IsActive = false;
        }
    }

    /// <summary>
    /// 恢复任务
    /// </summary>
    public void ResumeTask(string id)
    {
        if (taskDictionary.TryGetValue(id, out TimerTask task))
        {
            task.IsActive = true;
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
            return Mathf.Max(0, task.TriggerTime - UnityEngine.Time.time);
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

    /// <summary>
    /// 添加任务
    /// </summary>
    public string AddTask(float duration, Delegate callback, params object[] args)
    {
        var task = TimerTask.Create
        (
            duration,
            callback
        );

        taskQueue.Enqueue(task);
        taskDictionary[task.IdCallback] = task;

        return task.IdCallback;
    }
    #endregion
}

#region Priority Queue Implementation

#endregion