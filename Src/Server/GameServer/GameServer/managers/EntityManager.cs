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
        List<Entity> AllEntities = new List<Entity>();
        Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();

        public void AddEntity(int mapId, Entity entity)
        {
            AllEntities.Add(entity);
            entity.EntityData.Id = this.index++;

            List<Entity> entities = null;
            if(!MapEntities.TryGetValue(mapId, out entities))//out表示传引用
            {
                entities = new List<Entity>();
                MapEntities[mapId] = entities;   
            }
            
            entities.Add(entity);
        }

        public void RemoveEntity(int mapId, Entity entity)
        {
            AllEntities.Remove(entity);
            MapEntities[mapId].Remove(entity);
        }
    }
}
