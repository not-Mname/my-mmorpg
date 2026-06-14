using Common;
using Common.Data;
using GameServer.Managers.Data;
using GameServer.Managers.Entities;
using GameServer.Services.Battle;
using Network;
using SkillBridge.Message;

namespace GameServer.Models.Logic
{
    internal class Arena
    {
        public NArenaInfo ArenaInfo { get; private set; }
        public Map Map;
        public NetConnection<NetSession> Red;
        public NetConnection<NetSession> Blue;
        public  Map? SourceMapRed { get; private set; }
        public  Map? SourceMapBlue { get; private set; }
        public bool IsReady => _redReady && _blueReady;
        public ArenaStatus ArenaStatus = ArenaStatus.None;
        public ArenaRoundStatus ArenaRoundStatus = ArenaRoundStatus.None;
        public int Round = 0;

        private int _redSpawnPoint = 9;
        private int _blueSpawnPoint = 10;

        private bool _redReady = false;
        private bool _blueReady = false;

        private float _timer = 0.0f;

        private const float READY_TIME = 11.0f;
        private const float ROUND_TIME = 60.0f;
        private const float RESULT_TIME = 5.0f;

        public Arena(NArenaInfo arenaInfo, Map map, NetConnection<NetSession> red, NetConnection<NetSession> blue)
        {
            this.ArenaInfo = arenaInfo;
            this.Map = map;
            this.Red = red;
            this.Blue = blue;
            this.ArenaInfo.ArenaId = map.InstanceId;
            ArenaStatus = ArenaStatus.Wait;
            ArenaRoundStatus = ArenaRoundStatus.None;
        }

        public void Update()
        {
            if (this.ArenaStatus == ArenaStatus.Game)
            {
                UpdataRound();
            }
        }

        private void UpdataRound()
        {
            if (this.ArenaRoundStatus == ArenaRoundStatus.Ready)
            {// 预备阶段，倒计时结束后进入战斗阶段
                _timer -= Time.DeltaTime;
                if (_timer <= 0)
                {// 进入战斗阶段
                    this.ArenaRoundStatus = ArenaRoundStatus.Fight;
                    _timer = ROUND_TIME;
                    Log.Info($"Arena:[{this.ArenaInfo.ArenaId}] Round: {this.Round} Fight");
                    ArenaService.Instance.SendArenaRoundStart(this);
                }
            }
            else if (this.ArenaRoundStatus == ArenaRoundStatus.Fight)
            {// 战斗阶段，倒计时结束后进入结果阶段
                _timer -= Time.DeltaTime;
                if (_timer <= 0)
                {// 进入结果阶段
                    this.ArenaRoundStatus = ArenaRoundStatus.Result;
                    _timer = RESULT_TIME;
                    Log.Info($"Arena:[{this.ArenaInfo.ArenaId}] Round: {this.Round} Result");
                    ArenaService.Instance.SendArenaRoundEnd(this);
                }
            }
            else if (this.ArenaRoundStatus == ArenaRoundStatus.Result)
            {// 结果阶段，倒计时结束后进入下一轮或者结束比赛
                _timer -= Time.DeltaTime;
                if (_timer <= 0)
                {// 进入下一轮或者结束比赛
                    Log.Info($"Arena:[{this.ArenaInfo.ArenaId}] Round: {this.Round} End");

                    if (this.Round >= 3)
                    {// 比赛结束
                        Log.Info($"Arena:[{this.ArenaInfo.ArenaId}] End");
                        ArenaResult();
                    }
                    else
                    {// 进入下一轮
                        NextRound();
                    }

                }
            }
        }

        private void ArenaResult()
        {
            this.ArenaStatus = ArenaStatus.Result;
            // todo: 计算比赛结果，发送比赛结果给双方玩家
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
            EntityManager.Instance.RemoveMapEntity(currentMap.ID, currentMap.InstanceId, player.Session.Character);
            return currentMap;
        }

        internal void EntityReady(int entityId)
        {
            if (entityId == Red.Session.Character.EntityId)
            {
                _redReady = true;
            }
            else if (entityId == Blue.Session.Character.EntityId)
            {
                _blueReady = true;
            }

            if (IsReady)
            {
                ArenaStatus = ArenaStatus.Game;
                this.Round = 0;
                NextRound();
            }
        }

        private void NextRound()
        {
            this.Round++;
            this._timer = READY_TIME;
            this.ArenaRoundStatus = ArenaRoundStatus.Ready;
            Log.Info($"Arena:[{this.ArenaInfo.ArenaId}] Round: {this.Round} Ready");
            ArenaService.Instance.SendArenaReady(this);
        }
    }
}