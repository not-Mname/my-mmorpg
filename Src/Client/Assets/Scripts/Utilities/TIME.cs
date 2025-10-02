using System;

namespace Utilities
{
    public class TIME
    {
        public static string AddTask(float duration, Delegate callback, params object[] args)
        {
            return TimerManager.Instance.AddTask(duration, callback, args);
        }
    }
}
