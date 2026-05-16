using GameServer.Entities;
using GameServer.Models.Logic;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers.Entities
{
    class MonsterManager
    {
        private Map _map;

        public Dictionary<int, Monster> monsters = new Dictionary<int, Monster>();
        internal void Init(Map map)
        {
            this._map = map;
        }

        public Monster Create(int spawnMonId, int spawnMonLevel, NVector3 position, NVector3 direction)
        {
            Monster monster = new Monster(spawnMonId, spawnMonLevel, position, direction);
  
            EntityManager.Instance.AddEntity(this._map.ID, _map.InstanceId, monster);
            monster.Info.EntityId = monster.entityId;
            monster.Info.MapId = this._map.ID;
            this.monsters[monster.entityId] = monster;
            this._map.MonsterEnter(monster);
            return monster;
        }
    }
}
