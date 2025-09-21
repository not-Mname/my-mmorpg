using Const;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using Utilities;

namespace Managers
{
    class StatusService : Singleton<StatusService>, IDisposable
    {
        public delegate bool StatusNotifyHandler(NStatus status);
        Dictionary<StatusType ,StatusNotifyHandler> eventMap = new Dictionary<StatusType, StatusNotifyHandler>();
        HashSet<StatusNotifyHandler> handlers = new HashSet<StatusNotifyHandler>();
        public void RegiterStautsNotify(StatusType type, StatusNotifyHandler handler)
        {
            if(handlers.Contains(handler)) return;
            if(eventMap.ContainsKey(type))
                eventMap[type] += handler;
            else
                eventMap[type] = handler;
            handlers.Add(handler);
        }

        private void Notify(NStatus status)
        {
            LogHelper.LogFormat("StatusNotify: type:{0}, value:{1}, id:{2}, action:{3}", LogUser.StatusManager, status.Type, status.Value, status.Id, status.Action);

            if(status.Type == StatusType.Money)
            {
                EVENT.Fire(EventId.on_money_change);
                if(status.Action == StatusAction.Add)
                    User.Instance.AddGold(status.Value);
                else if(status.Action == StatusAction.Delete)
                    User.Instance.AddGold(-status.Value);
            }

            StatusNotifyHandler handler;
            if(eventMap.TryGetValue(status.Type, out handler))
            {
                handler(status);
            }
        }

        private void OnStatusNotify(object sender, StatusNotify status)
        {
            foreach(var statu in status.Status)
            {
                Notify(statu);
            }
        }

        public void Init()
        {
            MessageDistributer.Instance.Subscribe<StatusNotify>(OnStatusNotify);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(OnStatusNotify);
        }
    }
}
