using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
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

        public void PostProcess(NetMessageResponse response)
        {
            if(response.statusNotify == null) 
                response.statusNotify = new StatusNotify();

            foreach (var status in statuses)
            {
                response.statusNotify.Status.Add(status);
            }
            this.statuses.Clear();
        }
    }
}
