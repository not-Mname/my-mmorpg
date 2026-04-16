using Common.Data;
using Entities;
using Managers;
using MMO;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utilities;

namespace Services
{
    internal class MapService : Singleton<MapService>, IDisposable
    {
        public int CurrentMapId;

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }

        public void Init()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);

        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0} Count:{1}", response.mapId, response.Characters.Count);
            foreach (var cha in response.Characters)
            {
                if (User.Instance.CurrentCharacterInfo == null ||
                    (cha.Type == CharacterType.Player && (User.Instance.CurrentCharacterInfo.EntityId == cha.EntityId)))
                {
                    User.Instance.CurrentCharacterInfo = cha;
                    if (User.Instance.CurrentCharacter == null)
                        User.Instance.CurrentCharacter = new Character(cha);
                    else
                        User.Instance.CurrentCharacter.UpdateInfo(cha);
                    CharacterManager.Instance.AddCharacter(User.Instance.CurrentCharacter);
                    continue;
                }
                else if (cha.Type == CharacterType.Monster)
                {
                    CharacterManager.Instance.AddCharacter(new Monster(cha));
                    continue;
                }
                CharacterManager.Instance.AddCharacter(new Character(cha));
            }
            if (CurrentMapId != response.mapId)
            {
                this.EnterMap(response.mapId);
                this.CurrentMapId = response.mapId;
            }
        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse message)
        {
            Debug.LogFormat("OnMapCharacterLeave:Map:{0} CharacterId:{1}", User.Instance.CurrentMapData.ID, message.entityId);

            if (User.Instance.CurrentCharacterInfo != null && message.entityId != User.Instance.CurrentCharacterInfo.EntityId)
            {
                CharacterManager.Instance.RemoveCharacter(message.entityId);
            }
            else
            {
                CharacterManager.Instance.Clear();
                //User.Instance.CurrentCharacterInfo = null;
                //User.Instance.CurrentCharacterObject = null;
                //User.Instance.CurrentMapData = null;
            }

        }

        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);
                SoundManager.Instance.PlaySound(map.Music);
            }
            else
                LogHelper.LogErrorFormat("EnterMap: Map {0} not existed", LogUser.MapService, mapId);
        }

        private void OnMapEntitySync(object sender, MapEntitySyncResponse message)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("MapEntityUpdateResponse : Entitis:{0}", message.entitySyncs.Count);
            sb.AppendLine();
            foreach (var entity in message.entitySyncs)
            {
                EntityManager.Instance.OnEntitySync(entity);
                sb.AppendFormat("[{0}] event:{1} entity:{2}", entity.Id, entity.Event, entity.Entity.String());
                sb.AppendLine();
            }
            LogHelper.Log(sb.ToString(), LogUser.MapService);
        }

        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entityData, int param)
        {
            LogHelper.LogFormat("MapEntityUpdateRequest : ID:{0} POS:{1} DIR:{2} SPEED:{3} ", LogUser.MapService, entityData.Id, entityData.Position, entityData.Direction, entityData.Speed);
            NetMessage meg = new NetMessage();
            meg.Request = new NetMessageRequest();
            meg.Request.mapEntitySync = new MapEntitySyncRequest();

            meg.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Id = entityData.Id,
                Entity = entityData,
                Event = entityEvent,
                Param = param
            };

            NetClient.Instance.SendMessage(meg);
        }

        internal void SendMapTeleport(int id)
        {
            Debug.LogFormat("MapTeleportRequest : ID:{0}", id);
            NetMessage meg = new NetMessage();
            meg.Request = new NetMessageRequest();
            meg.Request.mapTeleport = new MapTeleportRequest();
            meg.Request.mapTeleport.teleporterId = id;
            NetClient.Instance.SendMessage(meg);
        }
    }
}
