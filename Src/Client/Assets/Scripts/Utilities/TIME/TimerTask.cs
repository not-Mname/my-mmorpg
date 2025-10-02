using System;
using System.Linq;
using UnityEngine;
using Utilities;

namespace Utilities.Time
{
    public class TimerTask : IComparable<TimerTask>, IDisposable
    {
        public string IdCallback;
        public string IdCallbackWithAllCheck;
        public float Duration;
        public float TriggerTime;
        public bool IsActive;
        public string EventName;
        public Delegate Callback;
        public object[] ArgsCallback;
        private TimerTask() { }

        public static TimerTask Create(float duration, Delegate callback, params object[] args)
        {
            TimerTask task = new TimerTask();
            task.IdCallback = Guid.NewGuid().ToString();
            task.Duration = duration;
            task.IsActive = true;
            task.TriggerTime = UnityEngine.Time.time + duration;
            task.Callback = callback;
            if (task.Callback != null)
            {
                EVENT.Subscribe(task.IdCallback, task.Callback as Action<object[]>);
            }
            return task;
        }

        // 实现IComparable用于优先队列排序
        public int CompareTo(TimerTask other)
        {
            return TriggerTime.CompareTo(other.TriggerTime);
        }

        public void FireCallback()
        {
            if (Callback != null)
            {
                EVENT.Fire(this.IdCallback, this.ArgsCallback);
            }
        }

        public void Dispose()
        {
            EVENT.Unsubscribe(IdCallback);

        }

        public float TimeRemaining
        {
            get { return Mathf.Max(0, TriggerTime - UnityEngine.Time.time); }
            private set { }
        }
    }
}
