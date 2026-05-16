using Common;
using GameServer.Core;
using GameServer.Entities;

namespace GameServer.Managers.Entities
{

    /// <summary>
    /// 实体管理器，管理所有实体，包括玩家，怪物等。
    /// 所有需要网络同步的实体都应该在这里注册。
    /// </summary>
    internal class EntityManager : Singleton<EntityManager>
    {
        /// <summary>
        /// 实体索引，用于生成唯一I，从1开始。
        /// </summary>
        private int index = 1;
        /// <summary>
        /// 所有实体，包括玩家和怪物。
        /// </summary>
        private Dictionary<int, Entity> _allEntities = new();
        /// <summary>
        /// 所有实体，按地图ID分组。
        /// </summary>
        private Dictionary<int, List<Entity>> _mapEntities = new();
        private readonly ReaderWriterLockSlim _rwLock = new();

        /// <summary>
        /// 副本应该在这里获取id
        /// 获取地图ID，格式为 mapId * 100000 + instanceId。
        /// 例如，地图ID为100的实例ID为1的地图ID为 10000001。
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public int GetMapId(int mapId,int instanceId)
        {
            return mapId * 100000 + instanceId;
        }

        public void AddEntity(int mapId, int instanceId, Entity entity)
        {
            _rwLock.EnterWriteLock();
            try
            {
                entity.EntityData.Id = Interlocked.Increment(ref index);
                _allEntities.Add(entity.entityId, entity);
                AddMapEntity(mapId, instanceId, entity);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public void AddMapEntity(int mapId, int instanceId, Entity entity)
        {
            int mapIdx = GetMapId(mapId, instanceId);
            if (!_mapEntities.TryGetValue(mapIdx, out var entities))
            {
                entities = new List<Entity>();
                _mapEntities[mapIdx] = entities;
            }
            entities.Add(entity);
        }

        public void RemoveEntity(int mapId, int instanceId, Entity entity)
        {
            _rwLock.EnterWriteLock();
            try
            {

                _allEntities.Remove(entity.entityId);
                RemoveMapEntity( mapId, instanceId, entity);
                
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        internal void RemoveMapEntity(int mapId, int instanceId, Entity entity)
        {
            int mapIdx = GetMapId(mapId, instanceId);
            if (_mapEntities.TryGetValue(mapIdx, out var entities))
            {
                entities.Remove(entity);
            }
        }

        public Entity GetEntity(int entityId)
        {
            _rwLock.EnterReadLock();
            try
            {
                _allEntities.TryGetValue(entityId, out var entity);
                return entity;
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        public BattleUnit GetUnit(int unitId)
        {
            return this.GetEntity(unitId) as BattleUnit;
        }

        /// <summary>
        /// 在指定地图ID的范围内查找实体，范围为圆形区域。
        /// </summary>
        /// <typeparam name="T">战斗单位</typeparam>
        /// <param name="id">地图id</param>
        /// <param name="match">筛选委托</param>
        /// <returns>符合条件的战斗单位</returns>
        public List<T> FindMapEntitiesInRange<T>(int id, int instanceId, Predicate<Entity> match) where T : BattleUnit
        {
            List<T> result = new ();
            if (match == null)
            {
                return result;
            }

            _rwLock.EnterReadLock();
            try
            {
                if (!_mapEntities.TryGetValue(GetMapId(id, instanceId), out var entities))
                {
                    return result;
                }
                foreach (var entity in entities)
                {
                    if (entity is T t && match(entity))
                    {
                        result.Add(t);
                    }
                }
                return result;
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        public List<T> FindMapEntitiesInRange<T>(int id, int instanceId, Vector3Int pos, int aoeRange) where T : BattleUnit
        {
            return FindMapEntitiesInRange<T>(id, instanceId, (Entity entity) =>
            {
                var unit = entity as BattleUnit;
                return unit.Distance(pos) < aoeRange;
            });
        }


    }
}
