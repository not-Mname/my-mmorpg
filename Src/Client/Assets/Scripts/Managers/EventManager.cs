using System;
using System.Collections.Generic;

public class EventManager : Singleton<EventManager>
{
    private Dictionary<string, Action<object[]>> _eventDictionary;


    public void Init()
    {
    }

    public void StartListening(string eventName, Action<object[]> listener)
    {
        Action<object[]> thisEvent;

        if (_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent += listener;
            _eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            _eventDictionary.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, Action<object[]> listener)
    {
        Action<object[]> thisEvent;
        if (_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            _eventDictionary[eventName] = thisEvent;
        }
    }

    public void TriggerEvent(string eventName, params object[] parameters)
    {
        Action<object[]> thisEvent;
        if (_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(parameters);
        }
    }
}