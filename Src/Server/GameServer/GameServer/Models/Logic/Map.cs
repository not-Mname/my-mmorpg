using SkillBridge.Message;
using Common;
using Common.Data;
using Network;
using GameServer.Entities;
using Common.Utils;
using GameServer.Managers.MBattle;
using GameServer.Managers.Social;
using GameServer.Managers.Entities;
using GameServer.Services.Entity;

namespace GameServer.Models.Logic
{
    /// <summary>
    /// 地图逻辑类，管理地图上的角色、怪物、战斗和出生逻辑
    /// </summary>
     class Map
    {
        /// <summary>
        /// 地图角色包装类，关联网络连接和角色对象
        /// </summary>
        public class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }

        /// <summary>
        /// 地图ID，取自配置定义
        /// </summary>
        public int ID
        {
            get { return this.Define.ID; }
        }
        public MapDefine Define;
        public GBattle.Battle Battle;

        /// <summary>
        /// 地图实例ID（支持同一地图多副本）
        /// </summary>
        public int InstanceId { get; private set; }

        /// <summary>
        /// 角色字典<角色Id，角色信息>
        /// </summary>
        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        SpawnManager spawnManager = new SpawnManager();
        public MonsterManager monsterManager = new MonsterManager();

        public Map(MapDefine define, int instanceId)
        {
            this.Define = define;
            this.spawnManager.Init(this);
            this.monsterManager.Init(this);
            this.Battle = new GBattle.Battle(this);
            this.InstanceId = instanceId;
        }

        /// <summary>
        /// 每帧更新，驱动出生逻辑和战斗逻辑
        /// </summary>
        public void Update()
        {
            spawnManager.Update();
            Battle.Update();
        }

        /// <summary>
        /// 角色进入地图，向该角色发送当前地图上已有角色和怪物的信息，并向其他角色广播新角色进入
        /// </summary>
        public void CharacterEnter(NetConnection<NetSession> sender, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Id);
            AddCharacter(sender, character);
            var enterRes = new MapCharacterEnterResponse() { MapId = this.Define.ID };

            // 将地图上已有的所有角色和怪物信息填充到响应中
            foreach (var kv in this.MapCharacters)
            {
                enterRes.Characters.Add(kv.Value.character.Info);
                if (kv.Value.character.Id != character.Id)
                {
                    this.AddCharacterEnterMap(kv.Value.connection, character.Info);
                }
            }
            foreach (var kv in this.monsterManager.monsters)
            {
                enterRes.Characters.Add(kv.Value.Info);
            }
            sender.Session.AddResponse(new NetMessageResponse { MapCharacterEnter = enterRes });
            sender.SendResponse();
        }

        public void AddCharacter(NetConnection<NetSession> sender, Character character)
        {
            character.OnEnterMap(this);
            if (!MapCharacters.ContainsKey(character.Id))
            {
                this.MapCharacters[character.Id] = new MapCharacter(sender, character);
            }

        }

        /// <summary>
        /// 角色离开地图，从字典移除并向所有其他角色广播离开消息
        /// </summary>
        public void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterLeave: Map:{0} characterId:{1}", this.Define.ID, character.Id);
            character.OnLeaveMap(this);
            foreach (var kv in this.MapCharacters)
            {
                this.SendCharacterLeaveMap(kv.Value.connection, character.Info);
            }

            this.MapCharacters.Remove(character.Id);

            foreach (var kv in this.monsterManager.monsters)
            {
                kv.Value.ClearTarget(character);
            }
        }

        /// <summary>
        /// 向指定连接发送角色进入地图的通知（用于广播给其他客户端）
        /// </summary>
        void AddCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            var enterRes = new MapCharacterEnterResponse();
            enterRes.MapId = this.Define.ID;
            enterRes.Characters.Add(character);
            conn.Session.AddResponse(new NetMessageResponse { MapCharacterEnter = enterRes });
            conn.SendResponse();
        }

        /// <summary>
        /// 向指定连接发送角色离开地图的通知
        /// </summary>
        public void SendCharacterLeaveMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            conn.Session.AddResponse(new NetMessageResponse { MapCharacterLeave = new MapCharacterLeaveResponse() { EntityId = character.EntityId } });
            if (conn.Session.Character.Guild != null)
                GuildManager.Instance.Guilds[conn.Session.Character.Guild.Id].timestamp = TimeUtil.timestamp;
            conn.SendResponse();
        }

        /// <summary>
        /// 更新实体同步信息（位置、速度、方向、坐骑状态等），并广播给其他玩家
        /// </summary>
        public void UpdateEntity(NEntitySync entitySync)
        {
            foreach (var kv in this.MapCharacters)
            {
                if (kv.Value.character.EntityId == entitySync.Entity.Id)
                {
                    kv.Value.character.Position = entitySync.Entity.Position;
                    kv.Value.character.Speed = entitySync.Entity.Speed;
                    kv.Value.character.Direction = entitySync.Entity.Direction;
                    if (entitySync.Event == EntityEvent.Ride)
                    {
                        kv.Value.character.Ride = entitySync.Param;
                    }
                    kv.Value.connection.SendResponse();
                }
                else
                {
                    MapService.Instance.SendEntityUpdate(kv.Value.connection, entitySync);
                }
            }
        }

        /// <summary>
        /// 怪物进入地图，初始化怪物所在地图并向所有角色广播怪物信息
        /// </summary>
        public void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter: Map:{0} monsterId:{1}", this.Define.ID, monster.EntityId);
            monster.OnEnterMap(this);
            foreach (var item in MapCharacters)
            {
                this.AddCharacterEnterMap(item.Value.connection, monster.Info);
            }
        }

        /// <summary>
        /// 广播战斗响应消息（技能释放、技能命中、Buff结果）给地图上的所有角色
        /// </summary>
        public void BroadcastBattleResponse(NetMessageResponse response)
        {
            foreach (var kv in MapCharacters)
            {
                if (response.SkillCast != null)
                {
                    kv.Value.connection.Session.AddResponse(new NetMessageResponse { SkillCast = response.SkillCast });
                }
                if (response.SkillHits != null)
                {
                    kv.Value.connection.Session.AddResponse(new NetMessageResponse { SkillHits = response.SkillHits });
                }
                if (response.BuffRes != null)
                {
                    kv.Value.connection.Session.AddResponse(new NetMessageResponse { BuffRes = response.BuffRes });
                }

                kv.Value.connection.SendResponse();
            }
        }
    }
}
