using GameServer.Entities;
using SkillBridge.Message;
using System.Collections.Generic;

namespace GameServer.Managers.Items
{
    class StatusManager
    {
        Character owner;

        List<NStatus> statuses { get; set; }

        public bool HasStatus { get { return statuses.Count > 0; } }

        public StatusManager(Character Owner)
        {
            this.owner = Owner;
            statuses = new List<NStatus>();
        }

        public void AddStatus(StatusType type, int id, int value, StatusAction action)
        {
            statuses.Add(new NStatus 
            { 
                Type = type,
                Id = id,
                Value = value,
                Action = action
            });
        }

        public void AddGlodChange(int goldDelta)
        {
            if(goldDelta > 0)
                AddStatus(StatusType.Money, 0, goldDelta, StatusAction.Add);
            if(goldDelta < 0)
                AddStatus(StatusType.Money, 0, -goldDelta, StatusAction.Delete);
        }
        
        public void AddItemChange(int id, int conut, StatusAction action)
        {
            AddStatus(StatusType.Item, id, conut, action);
        }

        public void PostProcess(NetMessage message)
        {
            if (!message.HasResponse(NetMessageResponse.PayloadOneofCase.StatusNotify))
            {
                var notify = new StatusNotify();
                foreach (var status in statuses)
                {
                    notify.Status.Add(status);
                }
                message.Responses.Add(new NetMessageResponse { StatusNotify = notify });
            }
            this.statuses.Clear();
        }

        public void AddExpChange(int exp)
        {
            AddStatus(StatusType.Exp, 0, exp, StatusAction.Add);
        }

        public void AddLevelUp(int level)
        {
            AddStatus(StatusType.Exp, 0, level, StatusAction.Add);
        }
    }
}
