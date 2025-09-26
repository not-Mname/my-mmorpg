using System;
using System.Collections.Generic;

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
        }
        else
        {
            _events.Add(eventName, listener);
        }
    }

    public void Unsubscribe(string eventName)
    {
        if (_events.ContainsKey(eventName))
        {
            _events.Remove(eventName);
        }
    }

    public void TriggerEvent(string eventName, params object[] parameters)
    {
        if (_events.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent?.Invoke(parameters);
        }
    }
}