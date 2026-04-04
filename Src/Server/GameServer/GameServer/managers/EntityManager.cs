using Common;
using GameServer.Core;
using GameServer.Entities;
using System;
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

        public List<T> FindMapEntitiesInRange<T>(int id, Predicate<Entity> match) where T : BattleUnit
        {
            if(match == null)
            {
                return null;
            }
            List<T> result = new List<T>();
            foreach (var entity in this._mapEntities[id])
            {
                if (entity is T && match(entity))
                {
                    result.Add(entity as T);
                }
            }
            return result;
        }

        public List<T> FindMapEntitiesInRange<T>(int id, Vector3Int pos, int aoeRange) where T : BattleUnit
        {
            return FindMapEntitiesInRange<T>(id, (Entity entity) =>
            {
                var unit = entity as BattleUnit;
                return unit.Distance(pos) < aoeRange;
            });
        }
    }
}
