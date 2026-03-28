using Common;
using Common.Data;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class Spawner
    {
        public SpawnRuleDefine Define { get; set; }
        public Map Map { get; set; }

        private SpawnPointDefine _spawnPoint;
        private bool _spawned;
        private float _unspawnTime = 0;

        public Spawner(SpawnRuleDefine define, Map map)
        {
            Define = define;
            Map = map;
            if (DataManager.Instance.SpawnPoints.ContainsKey(map.ID))
            {
                if (DataManager.Instance.SpawnPoints[map.ID].ContainsKey(define.ID))
                {
                    _spawnPoint = DataManager.Instance.SpawnPoints[Map.ID][Define.SpawnPoint];
                }
                else
                {
                    Log.ErrorFormat("SpawnPointDefine {0} not found in Map {1}", Define.SpawnPoint, Map.ID);
                }
            }
        }



        internal void Update()
        {
            if (this.CanSpawn())
            {
                this.Spawn();
            }
        }

        void Spawn()
        {
            this._spawned = true;
            Log.InfoFormat("Map[{0}] Spawn [Monster {1} Level {2}] at Point {3}", Map.ID, Define.ID, Define.SpawnLevel, _spawnPoint.Position.String());
            this.Map.monsterManager.Create(this.Define.SpawnMonID, this.Define.SpawnLevel, this._spawnPoint.Position, this._spawnPoint.Direction);
        }

        bool CanSpawn()
        {
            if(this._spawned)
                return false;
            if(this._unspawnTime + this.Define.SpawnPeriod > Time.time)
                return false;
            return true;
        }
    }
}
