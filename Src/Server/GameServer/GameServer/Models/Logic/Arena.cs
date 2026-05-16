using Common.Data;
using GameServer.Managers.Data;
using GameServer.Managers.Entities;
using Network;
using SkillBridge.Message;

namespace GameServer.Models.Logic
{
    internal class Arena
    {
        public NArenaInfo ArenaInfo{ get; private set; }
        public Map Map;
        public NetConnection<NetSession> Red;
        public NetConnection<NetSession> Blue;
        public Map SourceMapRed { get; private set; }
        public Map SourceMapBlue { get; private set; }

        private int _redSpawnPoint = 9;
        private int _blueSpawnPoint = 10;

        public Arena(NArenaInfo arenaInfo, Map map, NetConnection<NetSession> red, NetConnection<NetSession> blue)
        {
            this.ArenaInfo = arenaInfo;
            this.Map = map;
            this.Red = red;
            this.Blue = blue;
            this.ArenaInfo.ArenaId = map.InstanceId;
        }

        internal void PlayerEnter()
        {
            this.SourceMapRed = PlayerLeaveMap(this.Red);
            this.SourceMapBlue = PlayerLeaveMap(this.Blue);

            this.PlayerEnterArena();
        }

        private void PlayerEnterArena()
        {
            TeleporterDefine red = DataManager.Instance.Teleporters[_redSpawnPoint];
            this.Red.Session.Character.Position = red.Position;
            this.Red.Session.Character.Direction = red.Direction;

            TeleporterDefine blue = DataManager.Instance.Teleporters[_blueSpawnPoint];
            this.Blue.Session.Character.Position = blue.Position;
            this.Blue.Session.Character.Direction = blue.Direction;

            this.Map.AddCharacter(Red, this.Red.Session.Character);
            this.Map.AddCharacter(Blue, this.Blue.Session.Character);

            this.Map.CharacterEnter(Red, this.Red.Session.Character);
            this.Map.CharacterEnter(Blue, this.Blue.Session.Character);

            EntityManager.Instance.AddMapEntity(this.Map.ID, this.Map.InstanceId, this.Red.Session.Character);
            EntityManager.Instance.AddMapEntity(this.Map.ID, this.Map.InstanceId, this.Blue.Session.Character);
        }

        private Map PlayerLeaveMap(NetConnection<NetSession> player)
        {
            var currentMap = MapManager.Instance[player.Session.Character.Info.MapId];
            currentMap.CharacterLeave(player.Session.Character);
            EntityManager.Instance.RemoveMapEntity(currentMap.ID, currentMap.InstanceId, player.Session.Character );
            return currentMap;
        }
    }
}