using System;
using System.Collections.Generic;
using SkillBridge.Message;
using Common;
using Common.Data;
using Network;
using GameServer.Managers;
using GameServer.Entities;
using GameServer.Services;
using Common.Utils;

namespace GameServer.Models
{
    class Map
    {
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

        public int ID
        {
            get { return this.Define.ID; }
        }
        public MapDefine Define;
        public Battle.Battle Battle;

        /// <summary>
        /// 角色字典<角色Id，角色信息>
        /// </summary>
        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        SpawnManager spawnManager = new SpawnManager();
        public MonsterManager monsterManager = new MonsterManager();
        public Map(MapDefine define)
        {
            this.Define = define;
            this.spawnManager.Init(this);
            this.monsterManager.Init(this);
            this.Battle = new Battle.Battle(this);
        }

        public void Update()
        {
            spawnManager.Update();
            Battle.Update();
        }

        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="character"></param>
        public void CharacterEnter(NetConnection<NetSession> sender, Character character)
        {

            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            character.Info.mapId = this.ID;

            sender.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            sender.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
            this.MapCharacters[character.Id] = new MapCharacter(sender, character);

            foreach (var kv in this.MapCharacters)
            {
                sender.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                if (kv.Value.character.Id != character.Id)
                    this.AddCharacterEnterMap(kv.Value.connection, character.Info);
            }
            foreach (var kv in this.monsterManager.monsters)
            {
                sender.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }
            sender.SendResponse();
        }

        public void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterLeave: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            //EntityManager.Instance.RemoveEntity(character.Data.MapID, character);

            foreach (var kv in this.MapCharacters)
            {
                this.SendCharacterLeaveMap(kv.Value.connection, character.Info);
            }

            this.MapCharacters.Remove(character.Id);
        }

        void AddCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            if(conn.Session.Response.mapCharacterEnter == null)
            {
                conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
            }

            conn.Session.Response.mapCharacterEnter.Characters.Add(character);

            conn.SendResponse();
        }

        public void SendCharacterLeaveMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            conn.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            conn.Session.Response.mapCharacterLeave.entityId = character.EntityId;
            if (conn.Session.Character.Guild != null)
                GuildManager.Instance.Guilds[conn.Session.Character.Guild.Id].timestamp = TimeUtil.timestamp;
            conn.SendResponse();
        }

        public void UpdateEntity(NEntitySync entitySync)
        {
            foreach (var kv in this.MapCharacters)
            {
                if (kv.Value.character.entityId == entitySync.Entity.Id)
                {
                    kv.Value.character.Position = entitySync.Entity.Position;
                    kv.Value.character.Speed = entitySync.Entity.Speed;
                    kv.Value.character.Direction = entitySync.Entity.Direction;
                    if(entitySync.Event == EntityEvent.Ride)
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

        public void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter: Map:{0} monsterId:{1}", this.Define.ID, monster.entityId);
            monster.OnEnterMap(this);
            foreach (var item in MapCharacters)
            {
                this.AddCharacterEnterMap(item.Value.connection, monster.Info);
            }
        }

        public void BroadcastBattleResponse(NetMessageResponse response)
        {
            foreach (var kv in MapCharacters)
            {
                if(response.skillCast != null)
                {
                    kv.Value.connection.Session.Response.skillCast = response.skillCast;
                }
                if(response.skillHits != null)
                {
                    kv.Value.connection.Session.Response.skillHits = response.skillHits;
                }
                if(response.buffRes != null)
                {
                    kv.Value.connection.Session.Response.buffRes = response.buffRes;
                }

                kv.Value.connection.SendResponse();
            }
        }
    }
}
