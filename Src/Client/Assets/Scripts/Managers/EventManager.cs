// EventManager.cs
using System;
using System.Collections.Generic;
using Utilities;

public class EventManager : Singleton<EventManager>
{
    private Dictionary<string, Action<object[]>> _events;

    public void Init()
    {
        _events = new Dictionary<string, Action<object[]>>();
    }

    public void Subscribe(string eventName, Action<object[]> listener)
    {
        if (_events.ContainsKey(eventName))
        {
            // 如果已存在，替换掉旧的事件处理器
            _events[eventName] = listener;
            LogHelper.Log("EventManager: Event " + eventName + " has been replaced.", LogUser.EventManager);
        }
        else
        {
            _events.Add(eventName, listener);
            LogHelper.Log("EventManager: Event " + eventName + " has been subscribed.", LogUser.EventManager);
        }
    }

    public void Unsubscribe(string eventName)
    {
        if (_events.ContainsKey(eventName))
        {
            _events.Remove(eventName);
            LogHelper.Log("EventManager: Event " + eventName + " has been unsubscribed.", LogUser.EventManager);
        }
    }

    public void TriggerEvent(string eventName, params object[] parameters)
    {
        if (_events.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent?.Invoke(parameters);
            LogHelper.Log("EventManager: Event " + eventName + " has been triggered.", LogUser.EventManager);
        }
    }
}