using Common;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class EntityManager : Singleton<EntityManager>
    {
        int index = 0;
        Dictionary<int, Entity> AllEntities = new Dictionary<int, Entity>();
        Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();

        public void AddEntity(int mapId, Entity entity)
        {
            entity.EntityData.Id = this.index++;
            AllEntities.Add(entity.entityId, entity);
            List<Entity> entities = null;
            if (!MapEntities.TryGetValue(mapId, out entities))//out表示传引用
            {
                entities = new List<Entity>();
                MapEntities[mapId] = entities;
            }

            entities.Add(entity);
        }

        public void RemoveEntity(int mapId, Entity entity)
        {
            AllEntities.Remove(entity.entityId);
            MapEntities[mapId].Remove(entity);
        }

        public Entity GetEntity(int id)
        {
            Entity entity = null;
            this.AllEntities.TryGetValue(id, out entity);
            return entity;
        }

        public BattleUnit GetUnit(int id)
        {
            return this.GetEntity(id) as BattleUnit;
        }
    }
}
