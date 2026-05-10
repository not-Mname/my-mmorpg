using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// 日志分类枚举，用于控制不同模块的日志开关
    /// </summary>
    public enum LogUser
    {
        /// <summary>未分类 / 通用日志</summary>
        None,
        /// <summary>装备界面模块</summary>
        UIEquip,
        /// <summary>状态管理器模块</summary>
        StatusManager,
        /// <summary>角色管理器模块</summary>
        CharacterManager,
        /// <summary>数据管理器模块</summary>
        DataManager,
        /// <summary>地图服务模块</summary>
        MapService,
        /// <summary>战斗管理器模块</summary>
        BattleManager,
        /// <summary>战斗服务模块</summary>
        BattleService,
        /// <summary>输入管理器模块</summary>
        InputManager,
        /// <summary>事件管理器模块</summary>
        EventManager,
        /// <summary>战斗逻辑模块</summary>
        Battle,
    }

    /// <summary>
    /// 日志帮助类，提供按模块分类控制的日志输出功能。
    /// 内部基于 UnityEngine.Debug 实现，UnityLogger 会将其转发到 log4net。
    /// </summary>
    public static class LogHelper
    {
        private static readonly Dictionary<LogUser, bool> LogDic = new Dictionary<LogUser, bool>()
        {
            { LogUser.None, true },
            { LogUser.UIEquip, false },
            { LogUser.StatusManager, false },
            { LogUser.CharacterManager, false },
            { LogUser.DataManager, false },
            { LogUser.MapService, false },
            { LogUser.BattleManager, true },
            { LogUser.BattleService, true },
            { LogUser.InputManager, true },
            { LogUser.EventManager, true },
            { LogUser.Battle, true },
        };

        /// <summary>
        /// 检查指定分类的日志是否启用
        /// </summary>
        /// <param name="user">日志分类</param>
        /// <returns>是否启用</returns>
        public static bool IsEnabled(LogUser user)
        {
            return LogDic.TryGetValue(user, out bool isEnabled) && isEnabled;
        }

        /// <summary>
        /// 记录普通日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="user">日志分类（默认 None）</param>
        public static void Log(object message, LogUser user = LogUser.None)
        {
            if (!IsEnabled(user))
                return;

            Debug.Log(message?.ToString() ?? "null");
        }

        /// <summary>
        /// 记录格式化普通日志
        /// </summary>
        /// <param name="message">格式化字符串</param>
        /// <param name="user">日志分类（默认 None）</param>
        /// <param name="args">格式化参数</param>
        public static void LogFormat(string message, LogUser user = LogUser.None, params object[] args)
        {
            if (!IsEnabled(user))
                return;

            Debug.LogFormat(message, args ?? Array.Empty<object>());
        }

        /// <summary>
        /// 记录普通日志（含上下文对象）
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="context">日志关联的 Unity 对象</param>
        /// <param name="user">日志分类（默认 None）</param>
        public static void Log(object message, UnityEngine.Object context, LogUser user = LogUser.None)
        {
            if (!IsEnabled(user))
                return;

            Debug.Log(message?.ToString() ?? "null", context);
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="user">日志分类（默认 None）</param>
        public static void LogWarning(object message, LogUser user = LogUser.None)
        {
            if (!IsEnabled(user))
                return;

            Debug.LogWarning(message?.ToString() ?? "null");
        }

        /// <summary>
        /// 记录警告日志（含上下文对象）
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="context">日志关联的 Unity 对象</param>
        /// <param name="user">日志分类（默认 None）</param>
        public static void LogWarning(object message, UnityEngine.Object context, LogUser user = LogUser.None)
        {
            if (!IsEnabled(user))
                return;

            Debug.LogWarning(message?.ToString() ?? "null", context);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="user">日志分类（默认 None）</param>
        public static void LogError(object message, LogUser user = LogUser.None)
        {
            if (!IsEnabled(user))
                return;

            Debug.LogError(message?.ToString() ?? "null");
        }

        /// <summary>
        /// 记录错误日志（含上下文对象）
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="context">日志关联的 Unity 对象</param>
        /// <param name="user">日志分类（默认 None）</param>
        public static void LogError(object message, UnityEngine.Object context, LogUser user = LogUser.None)
        {
            if (!IsEnabled(user))
                return;

            Debug.LogError(message?.ToString() ?? "null", context);
        }

        /// <summary>
        /// 记录格式化错误日志
        /// </summary>
        /// <param name="message">格式化字符串</param>
        /// <param name="user">日志分类（默认 None）</param>
        /// <param name="args">格式化参数</param>
        public static void LogErrorFormat(string message, LogUser user = LogUser.None, params object[] args)
        {
            if (!IsEnabled(user))
                return;

            Debug.LogErrorFormat(message, args ?? Array.Empty<object>());
        }
    }
}
