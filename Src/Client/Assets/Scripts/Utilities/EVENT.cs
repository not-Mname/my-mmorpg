using Const;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class EVENT
    {
        // 存储已注册的委托，以便能够正确取消注册
        private static Dictionary<string, Dictionary<Delegate, Action<object[]>>> eventHandlers =
            new Dictionary<string, Dictionary<Delegate, Action<object[]>>>();

        public static void Subscribe(string eventName, Action callback)
        {
            if (EventManager.Instance == null || eventName == null || callback == null)
            {
                return;
            }

            Action<object[]> wrappedCallback = (args) => callback();
            RegisterHandler(eventName, callback, wrappedCallback);
            EventManager.Instance.StartListening(eventName, wrappedCallback);
        }

        public static void Subscribe(EventId eventName, Action callback)
        {
            if (EventManager.Instance == null || callback == null)
            {
                return;
            }

            string eventNameString = eventName.ToString();
            Action<object[]> wrappedCallback = (args) => callback();
            RegisterHandler(eventNameString, callback, wrappedCallback);
            EventManager.Instance.StartListening(eventNameString, wrappedCallback);
        }

        public static void Subscribe<T>(string eventName, Action<T> callback)
        {
            if (EventManager.Instance == null || eventName == null || callback == null)
            {
                return;
            }

            Action<object[]> wrappedCallback = (args) => callback((T)args[0]);
            RegisterHandler(eventName, callback, wrappedCallback);
            EventManager.Instance.StartListening(eventName, wrappedCallback);
        }

        public static void Subscribe<T>(EventId eventName, Action<T> callback)
        {
            if (EventManager.Instance == null || callback == null)
            {
                return;
            }

            string eventNameString = eventName.ToString();
            Action<object[]> wrappedCallback = (args) => callback((T)args[0]);
            RegisterHandler(eventNameString, callback, wrappedCallback);
            EventManager.Instance.StartListening(eventNameString, wrappedCallback);
        }

        public static void Unsubscribe(string eventName, Action callback)
        {
            if (EventManager.Instance == null || eventName == null || callback == null)
            {
                return;
            }

            var wrappedCallback = UnregisterHandler(eventName, callback);
            if (wrappedCallback != null)
            {
                EventManager.Instance.StopListening(eventName, wrappedCallback);
            }
        }

        public static void Unsubscribe(EventId eventName, Action callback)
        {
            if (EventManager.Instance == null || callback == null)
            {
                return;
            }

            string eventNameString = eventName.ToString();
            var wrappedCallback = UnregisterHandler(eventNameString, callback);
            if (wrappedCallback != null)
            {
                EventManager.Instance.StopListening(eventNameString, wrappedCallback);
            }
        }

        public static void Unsubscribe<T>(string eventName, Action<T> callback)
        {
            if (EventManager.Instance == null || eventName == null || callback == null)
            {
                return;
            }

            var wrappedCallback = UnregisterHandler(eventName, callback);
            if (wrappedCallback != null)
            {
                EventManager.Instance.StopListening(eventName, wrappedCallback);
            }
        }

        public static void Unsubscribe<T>(EventId eventName, Action<T> callback)
        {
            if (EventManager.Instance == null || callback == null)
            {
                return;
            }

            string eventNameString = eventName.ToString();
            var wrappedCallback = UnregisterHandler(eventNameString, callback);
            if (wrappedCallback != null)
            {
                EventManager.Instance.StopListening(eventNameString, wrappedCallback);
            }
        }

        private static void RegisterHandler(string eventName, Delegate originalCallback, Action<object[]> wrappedCallback)
        {
            if (!eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName] = new Dictionary<Delegate, Action<object[]>>();
            }

            eventHandlers[eventName][originalCallback] = wrappedCallback;
        }

        private static Action<object[]> UnregisterHandler(string eventName, Delegate originalCallback)
        {
            if (eventHandlers.ContainsKey(eventName) &&
                eventHandlers[eventName].ContainsKey(originalCallback))
            {
                var wrappedCallback = eventHandlers[eventName][originalCallback];
                eventHandlers[eventName].Remove(originalCallback);
                return wrappedCallback;
            }

            return null;
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
            string eventNameString = eventName.ToString();
            EventManager.Instance.TriggerEvent(eventNameString, args);
        }
    }
}