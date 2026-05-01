using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    interface IEntityNotify
    {
        void OnEntityRemoved();
        void OnEntityEvent(EntityEvent @event, int param);
        void OnEntityChange(NEntitySync entity);
    }

    class EntityManager : Singleton<EntityManager>
    {

        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();

        public void RigisterEntityChangeNotify(int entityId, IEntityNotify notifiers)
        {
            this.notifiers[entityId] = notifiers;
        }

        public void AddEntity(Entity entity)
        {
            entities[entity.EntityId] = entity;
        }

        public void RemoveEntity(NEntity entity)
        {
            if (!entities.ContainsKey(entity.Id)) return;

            entities.Remove(entity.Id);
            
            if (!notifiers.ContainsKey(entity.Id)) return;

            notifiers[entity.Id].OnEntityRemoved();
            notifiers.Remove(entity.Id);
        }

        public void OnEntitySync(NEntitySync entity)
        {
            Entity TempEntity;
            entities.TryGetValue(entity.Id, out TempEntity);
            if (TempEntity != null)
            {
                //Debug.Log("Entity Sync: " + entity.Entity.Position);
                TempEntity.EntityData = entity.Entity;
            }
            if (notifiers.ContainsKey(entity.Id))
            {
                notifiers[entity.Id].OnEntityEvent(entity.Event, entity.Param);
                notifiers[entity.Id].OnEntityChange(entity);
            }
        }

        public Entity GetEntity(int casterId)
        {
            Entity TempEntity = null;
            entities.TryGetValue(casterId, out TempEntity);
            return TempEntity;
        }
    }
}
