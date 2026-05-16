// EventManager.cs
using GameInterFace;
using System;
using System.Collections.Generic;
using UnityEngine.Scripting;
using Utilities;

/// <summary>
/// 核心事件管理器，采用单例模式
/// 提供基于字符串键的事件订阅、取消订阅和触发功能
/// 支持单播（EventMode.Unicast）和多播（EventMode.Multicast）两种模式
/// </summary>
[Preserve]
public class EventManager : Singleton<EventManager>, IInitializable
{
    /// <summary>
    /// 单播事件字典，key: 事件名称，value: 回调
    /// </summary>
    private Dictionary<string, Action<object[]>> _unicastEvents;

    /// <summary>
    /// 多播事件字典，key: 事件名称，value: 回调列表
    /// </summary>
    private Dictionary<string, List<Action<object[]>>> _multicastEvents;

    /// <summary>
    /// 初始化事件管理器
    /// </summary>
    public void Init()
    {
        _unicastEvents = new Dictionary<string, Action<object[]>>();
        _multicastEvents = new Dictionary<string, List<Action<object[]>>>();
    }

    /// <summary>
    /// 订阅事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="listener">事件处理器回调函数</param>
    /// <param name="mode">事件模式，默认为单播</param>
    public void Subscribe(string eventName, Action<object[]> listener, EventMode mode = EventMode.Unicast)
    {
        if (mode == EventMode.Unicast)
        {
            _unicastEvents[eventName] = listener;
        }
        else
        {
            if (!_multicastEvents.TryGetValue(eventName, out var list))
            {
                list = new List<Action<object[]>>();
                _multicastEvents[eventName] = list;
            }
            list.Add(listener);
        }
    }

    /// <summary>
    /// 取消订阅事件（单播时忽略 listener 参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="listener">多播时指定要移除的监听器</param>
    /// <param name="mode">事件模式，默认为单播</param>
    public void Unsubscribe(string eventName, Action<object[]> listener = null, EventMode mode = EventMode.Unicast)
    {
        if (mode == EventMode.Unicast)
        {
            _unicastEvents.Remove(eventName);
        }
        else
        {
            if (listener != null && _multicastEvents.TryGetValue(eventName, out var list))
            {
                list.Remove(listener);
                if (list.Count == 0)
                {
                    _multicastEvents.Remove(eventName);
                }
            }
        }
    }

    /// <summary>
    /// 触发单播事件（向后兼容）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="parameters">事件参数</param>
    public void TriggerEvent(string eventName, params object[] parameters)
    {
        if (_unicastEvents.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent?.Invoke(parameters);
        }
    }

    /// <summary>
    /// 触发指定模式的事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="mode">事件模式</param>
    /// <param name="parameters">事件参数</param>
    public void TriggerEvent(string eventName, EventMode mode, params object[] parameters)
    {
        if (mode == EventMode.Unicast)
        {
            if (_unicastEvents.TryGetValue(eventName, out var handler))
            {
                handler?.Invoke(parameters);
            }
        }
        else
        {
            if (_multicastEvents.TryGetValue(eventName, out var list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    try
                    {
                        list[i]?.Invoke(parameters);
                    }
                    catch (System.Exception ex)
                    {
                        // 单个监听器异常不影响其他监听器
                    }
                }
            }
        }
    }
}
