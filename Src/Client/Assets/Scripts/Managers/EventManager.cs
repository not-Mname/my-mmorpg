// EventManager.cs
using System;
using System.Collections.Generic;

public class EventManager : Singleton<EventManager>
{
    // 合并后的字典，存储事件名到事件处理器映射的映射
    private Dictionary<string, EventHandlerInfo> _eventHandlers;

    public void Init()
    {
        _eventHandlers = new Dictionary<string, EventHandlerInfo>();
    }

    // 内部类，存储事件处理器的信息
    private class EventHandlerInfo
    {
        public Action<object[]> EventAction { get; set; }
        public Dictionary<Delegate, Action<object[]>> HandlerMap { get; } = new Dictionary<Delegate, Action<object[]>>();
    }

    public void Subscribe(string eventName, Delegate originalCallback, Action<object[]> wrappedCallback)
    {
        if (!_eventHandlers.TryGetValue(eventName, out var handlerInfo))
        {
            handlerInfo = new EventHandlerInfo();
            _eventHandlers.Add(eventName, handlerInfo);
        }

        // 添加处理器映射
        handlerInfo.HandlerMap[originalCallback] = wrappedCallback;

        // 更新事件委托
        handlerInfo.EventAction += wrappedCallback;
    }

    public void Unsubscribe(string eventName, Delegate originalCallback)
    {
        if (_eventHandlers.TryGetValue(eventName, out var handlerInfo) &&
            handlerInfo.HandlerMap.TryGetValue(originalCallback, out var wrappedCallback))
        {
            // 从事件委托中移除
            handlerInfo.EventAction -= wrappedCallback;

            // 从映射中移除
            handlerInfo.HandlerMap.Remove(originalCallback);

            // 如果没有处理器了，移除整个事件
            if (handlerInfo.HandlerMap.Count == 0)
            {
                _eventHandlers.Remove(eventName);
            }
        }
    }

    public void TriggerEvent(string eventName, params object[] parameters)
    {
        if (_eventHandlers.TryGetValue(eventName, out var handlerInfo))
        {
            handlerInfo.EventAction?.Invoke(parameters);
        }
    }
}