using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public enum LogUser
    {
        None,
        UIEquip,
        StatusManager,
        CharacterManager,
        DataManager,
        MapService,
        BattleManager,
        BattleService,
    }

    public class LogHelper
    {
        public static void LogShowJson(object message, LogUser user = LogUser.None)
        {
            string json = JsonConvert.SerializeObject(message);
            if (LogDic.TryGetValue(user, out bool isEnabled) && isEnabled)
            {
                Debug.Log(json);
            }
        }

        public static void Log(object message, LogUser user = LogUser.None)
        {
            if (LogDic.TryGetValue(user, out bool isEnabled) && isEnabled)
            {
                Debug.Log(message);
            }
        }

        public static void LogFormat(string message, LogUser user = LogUser.None, params object[] args)
        {
            if (LogDic.TryGetValue(user, out bool isEnabled) && isEnabled)
            {
                Debug.LogFormat(message, args);
            }
        }

        public static void Log(object message, UnityEngine.Object context, LogUser user = LogUser.None)
        {
            if (LogDic.TryGetValue(user, out bool isEnabled) && isEnabled)
            {
                Debug.Log(message, context);
            }
        }

        public static void LogWarning(object message, LogUser user = LogUser.None)
        {
            if (LogDic.TryGetValue(user, out bool isEnabled) && isEnabled)
            {
                Debug.LogWarning(message);
            }
        }

        public static void LogWarning(object message, UnityEngine.Object context, LogUser user = LogUser.None)
        {
            if (LogDic.TryGetValue(user, out bool isEnabled) && isEnabled)
            {
                Debug.LogWarning(message, context);
            }
        }

        public static void LogError(object message, LogUser user = LogUser.None)
        {
            if (LogDic.TryGetValue(user, out bool isEnabled) && isEnabled)
            {
                Debug.LogError(message);
            }
        }

        public static void LogError(object message, UnityEngine.Object context, LogUser user = LogUser.None)
        {
            if (LogDic.TryGetValue(user, out bool isEnabled) && isEnabled)
            {
                Debug.LogError(message, context);
            }
        }

        public static void LogErrorFormat(string message, LogUser user = LogUser.None, params object[] args)
        {
            if (LogDic.TryGetValue(user, out bool isEnabled) && isEnabled)
            {
                Debug.LogErrorFormat(message, args);
            }
        }

        public static Dictionary<LogUser, bool> LogDic = new Dictionary<LogUser, bool>() 
        { 
            { LogUser.None, true },
            { LogUser.UIEquip, false },
            { LogUser.StatusManager, false },
            { LogUser.CharacterManager, false },
            { LogUser.DataManager, false },
            { LogUser.MapService, false },
            { LogUser.BattleManager, true },
            { LogUser.BattleService, true },
        };
    }
}
