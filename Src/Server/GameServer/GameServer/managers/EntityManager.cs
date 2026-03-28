using Common;
using GameServer.Entities;
using System.Collections.Generic;

namespace GameServer.Managers
{

    /// <summary>
    /// 实体管理器，管理所有实体，包括玩家，怪物等。
    /// 所有需要网络同步的实体都应该在这里注册。
    /// </summary>
    internal class EntityManager : Singleton<EntityManager>
    {
        private int index = 1;
        private Dictionary<int, Entity> _allEntities = new Dictionary<int, Entity>();
        private Dictionary<int, List<Entity>> _mapEntities = new Dictionary<int, List<Entity>>();

        public void AddEntity(int mapId, Entity entity)
        {
            entity.EntityData.Id = this.index++;
            _allEntities.Add(entity.entityId, entity);
            List<Entity> entities = null;
            if (!_mapEntities.TryGetValue(mapId, out entities))
            {
                entities = new List<Entity>();
                _mapEntities[mapId] = entities;
            }

            entities.Add(entity);
        }

        public void RemoveEntity(int mapId, Entity entity)
        {
            _allEntities.Remove(entity.entityId);
            _mapEntities[mapId].Remove(entity);
        }

        public Entity GetEntity(int id)
        {
            Entity entity = null;
            this._allEntities.TryGetValue(id, out entity);
            return entity;
        }

        public BattleUnit GetUnit(int id)
        {
            return this.GetEntity(id) as BattleUnit;
        }
    }
}
