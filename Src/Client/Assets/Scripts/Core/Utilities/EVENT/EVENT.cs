using Const;
using System;
using System.Collections.Generic;

namespace Utilities
{
    /// <summary>
    /// 事件系统工具类，提供类型安全的事件订阅和触发接口
    /// 封装底层EventManager，简化事件操作并支持泛型参数
    /// </summary>
    public class EVENT
    {
        /// <summary>
        /// 多播订阅时，记录原始回调到包装回调的映射，用于取消订阅时查找
        /// </summary>
        private static Dictionary<Delegate, Action<object[]>> _multicastWrappers = new Dictionary<Delegate, Action<object[]>>();
        /// <summary>
        /// 订阅无参数事件（使用字符串事件名）
        /// </summary>
        /// <param name="eventName">事件名称字符串</param>
        /// <param name="callback">无参数回调函数</param>
        public static void Subscribe(string eventName, Action callback)
        {
            if (EventManager.Instance == null || eventName == null || callback == null)
            {
                return;
            }

            // 包装无参数回调为对象数组参数格式
            Action<object[]> wrappedCallback = (args) => callback();
            EventManager.Instance.Subscribe(eventName, wrappedCallback);
        }

        /// <summary>
        /// 订阅无参数事件（使用EventId枚举）
        /// </summary>
        /// <param name="eventName">事件ID枚举值</param>
        /// <param name="callback">无参数回调函数</param>
        public static void Subscribe(EventId eventName, Action callback)
        {
            if (EventManager.Instance == null || callback == null)
            {
                return;
            }

            // 直接将枚举转换为字符串
            string eventNameString = eventName.ToString();
            Action<object[]> wrappedCallback = (args) => callback();
            EventManager.Instance.Subscribe(eventNameString, wrappedCallback);
        }

        /// <summary>
        /// 订阅单参数事件（使用字符串事件名）
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="eventName">事件名称字符串</param>
        /// <param name="callback">单参数回调函数</param>
        public static void Subscribe<T>(string eventName, Action<T> callback)
        {
            if (EventManager.Instance == null || eventName == null || callback == null)
            {
                return;
            }

            // 包装单参数回调，从对象数组提取第一个参数
            Action<object[]> wrappedCallback = (args) => callback((T)args[0]);
            EventManager.Instance.Subscribe(eventName, wrappedCallback);
        }

        /// <summary>
        /// 订阅单参数事件（使用EventId枚举）
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="eventName">事件ID枚举值</param>
        /// <param name="callback">单参数回调函数</param>
        public static void Subscribe<T>(EventId eventName, Action<T> callback)
        {
            if (EventManager.Instance == null || callback == null)
            {
                return;
            }

            string eventNameString = eventName.ToString();
            Action<object[]> wrappedCallback = (args) => callback((T)args[0]);
            EventManager.Instance.Subscribe(eventNameString, wrappedCallback);
        }

        /// <summary>
        /// 订阅双参数事件（使用字符串事件名）
        /// </summary>
        /// <typeparam name="T1">第一个参数类型</typeparam>
        /// <typeparam name="T2">第二个参数类型</typeparam>
        /// <param name="eventName">事件名称字符串</param>
        /// <param name="callback">双参数回调函数</param>
        public static void Subscribe<T1, T2>(string eventName, Action<T1, T2> callback)
        {
            if (EventManager.Instance == null || eventName == null || callback == null)
            {
                return;
            }

            // 包装双参数回调，从对象数组提取前两个参数
            Action<object[]> wrappedCallback = (args) => callback((T1)args[0], (T2)args[1]);
            EventManager.Instance.Subscribe(eventName, wrappedCallback);
        }

        /// <summary>
        /// 订阅双参数事件（使用EventId枚举）
        /// </summary>
        /// <typeparam name="T1">第一个参数类型</typeparam>
        /// <typeparam name="T2">第二个参数类型</typeparam>
        /// <param name="eventName">事件ID枚举值</param>
        /// <param name="callback">双参数回调函数</param>
        public static void Subscribe<T1, T2>(EventId eventName, Action<T1, T2> callback)
        {
            if (EventManager.Instance == null || callback == null)
            {
                return;
            }

            string eventNameString = eventName.ToString();
            Action<object[]> wrappedCallback = (args) => callback((T1)args[0], (T2)args[1]);
            EventManager.Instance.Subscribe(eventNameString, wrappedCallback);
        }

        // ===== 多播模式 Subscribe 重载 =====

        /// <summary>
        /// 订阅无参数事件
        /// </summary>
        public static void Subscribe(string eventName, Action callback, EventMode mode)
        {
            if (EventManager.Instance == null || eventName == null || callback == null) return;

            Action<object[]> wrappedCallback = (args) => callback();
            if (mode == EventMode.Multicast) _multicastWrappers[callback] = wrappedCallback;
            EventManager.Instance.Subscribe(eventName, wrappedCallback, mode);
        }

        /// <summary>
        /// 订阅无参数事件
        /// </summary>
        public static void Subscribe(EventId eventName, Action callback, EventMode mode)
        {
            if (EventManager.Instance == null || callback == null) return;

            string eventNameString = eventName.ToString();
            Action<object[]> wrappedCallback = (args) => callback();
            if (mode == EventMode.Multicast) _multicastWrappers[callback] = wrappedCallback;
            EventManager.Instance.Subscribe(eventNameString, wrappedCallback, mode);
        }

        /// <summary>
        /// 订阅单参数事件
        /// </summary>
        public static void Subscribe<T>(string eventName, Action<T> callback, EventMode mode)
        {
            if (EventManager.Instance == null || eventName == null || callback == null) return;

            Action<object[]> wrappedCallback = (args) => callback((T)args[0]);
            if (mode == EventMode.Multicast) _multicastWrappers[callback] = wrappedCallback;
            EventManager.Instance.Subscribe(eventName, wrappedCallback, mode);
        }

        /// <summary>
        /// 订阅单参数事件
        /// </summary>
        public static void Subscribe<T>(EventId eventName, Action<T> callback, EventMode mode)
        {
            if (EventManager.Instance == null || callback == null) return;

            string eventNameString = eventName.ToString();
            Action<object[]> wrappedCallback = (args) => callback((T)args[0]);
            if (mode == EventMode.Multicast) _multicastWrappers[callback] = wrappedCallback;
            EventManager.Instance.Subscribe(eventNameString, wrappedCallback, mode);
        }

        /// <summary>
        /// 订阅双参数事件
        /// </summary>
        public static void Subscribe<T1, T2>(string eventName, Action<T1, T2> callback, EventMode mode)
        {
            if (EventManager.Instance == null || eventName == null || callback == null) return;

            Action<object[]> wrappedCallback = (args) => callback((T1)args[0], (T2)args[1]);
            if (mode == EventMode.Multicast) _multicastWrappers[callback] = wrappedCallback;
            EventManager.Instance.Subscribe(eventName, wrappedCallback, mode);
        }

        /// <summary>
        /// 订阅双参数事件
        /// </summary>
        public static void Subscribe<T1, T2>(EventId eventName, Action<T1, T2> callback, EventMode mode)
        {
            if (EventManager.Instance == null || callback == null) return;

            string eventNameString = eventName.ToString();
            Action<object[]> wrappedCallback = (args) => callback((T1)args[0], (T2)args[1]);
            if (mode == EventMode.Multicast) _multicastWrappers[callback] = wrappedCallback;
            EventManager.Instance.Subscribe(eventNameString, wrappedCallback, mode);
        }

        /// <summary>
        /// 取消订阅事件（使用EventId枚举）
        /// </summary>
        /// <param name="eventName">要取消订阅的事件ID</param>
        public static void Unsubscribe(EventId eventName)
        {
            if (EventManager.Instance == null)
            {
                return;
            }

            string eventNameString = eventName.ToString();
            EventManager.Instance.Unsubscribe(eventNameString);
        }

        /// <summary>
        /// 取消订阅事件（使用字符串事件名）
        /// </summary>
        /// <param name="eventName">要取消订阅的事件名称</param>
        public static void Unsubscribe(string eventName)
        {
            if (EventManager.Instance == null || eventName == null)
            {
                return;
            }

            EventManager.Instance.Unsubscribe(eventName);
        }

        // ===== 多播模式 Unsubscribe 重载 =====

        /// <summary>
        /// 取消订阅无参数事件
        /// </summary>
        public static void Unsubscribe(string eventName, Action callback, EventMode mode)
        {
            if (EventManager.Instance == null || eventName == null) return;
            if (mode == EventMode.Multicast && _multicastWrappers.TryGetValue(callback, out var wrapped))
            {
                EventManager.Instance.Unsubscribe(eventName, wrapped, mode);
                _multicastWrappers.Remove(callback);
            }
            else
            {
                EventManager.Instance.Unsubscribe(eventName, null, mode);
            }
        }

        /// <summary>
        /// 取消订阅无参数事件
        /// </summary>
        public static void Unsubscribe(EventId eventName, Action callback, EventMode mode)
        {
            if (EventManager.Instance == null) return;
            if (mode == EventMode.Multicast && _multicastWrappers.TryGetValue(callback, out var wrapped))
            {
                EventManager.Instance.Unsubscribe(eventName.ToString(), wrapped, mode);
                _multicastWrappers.Remove(callback);
            }
            else
            {
                EventManager.Instance.Unsubscribe(eventName.ToString(), null, mode);
            }
        }

        /// <summary>
        /// 取消订阅单参数事件
        /// </summary>
        public static void Unsubscribe<T>(string eventName, Action<T> callback, EventMode mode)
        {
            if (EventManager.Instance == null || eventName == null) return;
            if (mode == EventMode.Multicast && _multicastWrappers.TryGetValue(callback, out var wrapped))
            {
                EventManager.Instance.Unsubscribe(eventName, wrapped, mode);
                _multicastWrappers.Remove(callback);
            }
            else
            {
                EventManager.Instance.Unsubscribe(eventName, null, mode);
            }
        }

        /// <summary>
        /// 取消订阅单参数事件
        /// </summary>
        public static void Unsubscribe<T>(EventId eventName, Action<T> callback, EventMode mode)
        {
            if (EventManager.Instance == null) return;
            if (mode == EventMode.Multicast && _multicastWrappers.TryGetValue(callback, out var wrapped))
            {
                EventManager.Instance.Unsubscribe(eventName.ToString(), wrapped, mode);
                _multicastWrappers.Remove(callback);
            }
            else
            {
                EventManager.Instance.Unsubscribe(eventName.ToString(), null, mode);
            }
        }

        /// <summary>
        /// 取消订阅双参数事件
        /// </summary>
        public static void Unsubscribe<T1, T2>(string eventName, Action<T1, T2> callback, EventMode mode)
        {
            if (EventManager.Instance == null || eventName == null) return;
            if (mode == EventMode.Multicast && _multicastWrappers.TryGetValue(callback, out var wrapped))
            {
                EventManager.Instance.Unsubscribe(eventName, wrapped, mode);
                _multicastWrappers.Remove(callback);
            }
            else
            {
                EventManager.Instance.Unsubscribe(eventName, null, mode);
            }
        }

        /// <summary>
        /// 取消订阅双参数事件
        /// </summary>
        public static void Unsubscribe<T1, T2>(EventId eventName, Action<T1, T2> callback, EventMode mode)
        {
            if (EventManager.Instance == null) return;
            if (mode == EventMode.Multicast && _multicastWrappers.TryGetValue(callback, out var wrapped))
            {
                EventManager.Instance.Unsubscribe(eventName.ToString(), wrapped, mode);
                _multicastWrappers.Remove(callback);
            }
            else
            {
                EventManager.Instance.Unsubscribe(eventName.ToString(), null, mode);
            }
        }

        /// <summary>
        /// 触发事件（使用字符串事件名）
        /// </summary>
        /// <param name="eventName">要触发的事件名称</param>
        /// <param name="args">事件参数数组</param>
        public static void Fire(string eventName, params object[] args)
        {
            if (EventManager.Instance == null || eventName == null)
            {
                return;
            }
            EventManager.Instance.TriggerEvent(eventName, args);
        }

        /// <summary>
        /// 触发事件（使用EventId枚举）
        /// </summary>
        /// <param name="eventName">要触发的事件ID</param>
        /// <param name="args">事件参数数组</param>
        public static void Fire(EventId eventName, params object[] args)
        {
            if (EventManager.Instance == null)
            {
                return;
            }
            string eventNameString = eventName.ToString();
            EventManager.Instance.TriggerEvent(eventNameString, args);
        }

        // ===== 多播模式 Fire 重载 =====

        /// <summary>
        /// 触发指定模式的事件
        /// </summary>
        public static void Fire(string eventName, EventMode mode, params object[] args)
        {
            if (EventManager.Instance == null || eventName == null) return;
            EventManager.Instance.TriggerEvent(eventName, mode, args);
        }

        /// <summary>
        /// 触发指定模式的事件
        /// </summary>
        public static void Fire(EventId eventName, EventMode mode, params object[] args)
        {
            if (EventManager.Instance == null) return;
            EventManager.Instance.TriggerEvent(eventName.ToString(), mode, args);
        }
    }
}