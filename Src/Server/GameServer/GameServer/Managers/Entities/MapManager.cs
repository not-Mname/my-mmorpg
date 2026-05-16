using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Data;
using GameServer.Managers.Data;
using GameServer.Models.Logic;

namespace GameServer.Managers.Entities
{
    class MapManager : Singleton<MapManager>
    {
        /// <summary>
        /// 地图实例字典，key为mapid, value为该地图的所有副本实例
        /// </summary>
        Dictionary<int, Dictionary<int, Map>> Maps = new();

        public void Init()
        {
            Log.Info("MapManage Init...");
            foreach (var mapdefine in DataManager.Instance.Maps.Values)
            {
                int instance = 1;
                if(mapdefine.Type == MapType.Arena)
                {
                    instance = 100;
                }
                this.Maps[mapdefine.ID] = new();
                Log.Info($"MapManager.Init > Map:{mapdefine.ID}:{mapdefine.Name}, Instance:{instance}");
                for (int i = 0; i < instance; i++)
                {
                    Map map = new Map(mapdefine, i);
                    this.Maps[mapdefine.ID][i] = map;
                }
                
            }
        }


        /// <summary>
        /// 获取非副本地图实例
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Map this[int key]
        {
            get
            {
                return this.Maps[key][0];
            }
        }

        public void Update()
        {
            foreach(var maps in this.Maps.Values)
            {
                foreach(var mapInstance in maps.Values)
                {
                    mapInstance.Update();
                }
            }
        }

        public Map GetInstance(int mapId, int instanceId)
        {
            return this.Maps[mapId][instanceId];
        }
    }
}
