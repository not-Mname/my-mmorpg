// EVENT.cs
using Const;
using System;
using UnityEngine;

namespace Utilities
{
    public class EVENT
    {
        public static void Subscribe(string eventName, Action callback)
        {
            if (EventManager.Instance == null || eventName == null || callback == null)
            {
                return;
            }

            Action<object[]> wrappedCallback = (args) => callback();
            EventManager.Instance.Subscribe(eventName, wrappedCallback);
        }

        public static void Subscribe(EventId eventName, Action callback)
        {
            if (EventManager.Instance == null || callback == null)
            {
                return;
            }

            string eventNameString = eventName.ToString();
            Action<object[]> wrappedCallback = (args) => callback();
            EventManager.Instance.Subscribe(eventNameString, wrappedCallback);
        }

        public static void Subscribe<T>(string eventName, Action<T> callback)
        {
            if (EventManager.Instance == null || eventName == null || callback == null)
            {
                return;
            }

            Action<object[]> wrappedCallback = (args) => callback((T)args[0]);
            EventManager.Instance.Subscribe(eventName, wrappedCallback);
        }

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

        public static void Unsubscribe(EventId eventName)
        {
            if (EventManager.Instance == null)
            {
                return;
            }

            string eventNameString = eventName.ToString();
            EventManager.Instance.Unsubscribe(eventNameString);
        }

        public static void Unsubscribe(string eventName)
        {
            if (EventManager.Instance == null || eventName == null)
            {
                return;
            }

            EventManager.Instance.Unsubscribe(eventName);
        }

        public static void Fire(string eventName, params object[] args)
        {
            if (EventManager.Instance == null || eventName == null)
            {
                return;
            }
            EventManager.Instance.TriggerEvent(eventName, args);
        }

        public static void Fire(EventId eventName, params object[] args)
        {
            if (EventManager.Instance == null)
            {
                return;
            }
            LogHelper.LogShowJson(args);
            string eventNameString = eventName.ToString();
            EventManager.Instance.TriggerEvent(eventNameString, args);
        }
    }
}