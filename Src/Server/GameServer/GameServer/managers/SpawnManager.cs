using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class SpawnManager
    {
        public List<Spawner> rules = new List<Spawner>();
        private Map map;
        internal void Init(Map _map)
        {
            this.map = _map;
            if (DataManager.Instance.SpawnRules.ContainsKey(map.ID))
            {
                foreach (var define in DataManager.Instance.SpawnRules[map.ID].Values)
                {
                    rules.Add(new Spawner(define, this.map));
                }
            }

        }

        internal void Update()
        {
            if (rules.Count == 0) return;
            for(int i = 0; i < rules.Count; i++)
            {
                rules[i].Update();
            }
        }
    }
}
