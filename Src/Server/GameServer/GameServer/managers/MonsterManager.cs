using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
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
  
            EntityManager.Instance.AddEntity(this._map.ID, monster);
            monster.Info.EntityId = monster.entityId;
            monster.Info.mapId = this._map.ID;
            this.monsters[monster.Id] = monster;
            this._map.MonsterEnter(monster);
            return monster;
        }
    }
}
