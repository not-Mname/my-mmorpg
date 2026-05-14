using Asset;
using Common.Data;
using Entities;
using GameInterFace;
using Managers;
using MMO;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Text;
using UnityEngine;
using Utilities;

namespace Services
{
    internal class MapService : Singleton<MapService>, IDisposable, IInitializable
    {
        public int CurrentMapId;
        public bool MapSystemInitDone = false;
        private bool _isLoadingDone = true;

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
            SceneManager.Instance.OnSenceLoadDone -= this.OnSceneLoadDone;
        }

        public void Init()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
            SceneManager.Instance.OnSenceLoadDone += this.OnSceneLoadDone;

        }

        private void OnSceneLoadDone()
        {
            if (User.Instance.CurrentCharacter != null)
            {
                User.Instance.CurrentCharacter.Ready = true;
            }
            if (User.Instance.CurrentCharacterObject != null)
            {
                User.Instance.CurrentCharacterObject.OnEnterMap();
            }
            this._isLoadingDone = true;
        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0} Count:{1}", response.MapId, response.Characters.Count);
            foreach (var cha in response.Characters)
            {
                if (User.Instance.CurrentCharacterInfo == null ||
                    (cha.Type == CharacterType.Player && (User.Instance.CurrentCharacterInfo.EntityId == cha.EntityId)))
                {// 玩家自己
                    User.Instance.CurrentCharacterInfo = cha;
                    if (User.Instance.CurrentCharacter == null)
                    { User.Instance.CurrentCharacter = new Character(cha); }
                    else
                    { User.Instance.CurrentCharacter.UpdateInfo(cha); }
                    User.Instance.CurrentCharacter.Ready = false;
                    User.Instance.Init();
                    CharacterManager.Instance.AddCharacter(User.Instance.CurrentCharacter);
                    if (CurrentMapId != response.MapId)
                    {
                        this.EnterMap(response.MapId);
                        this.CurrentMapId = response.MapId;
                    }
                    continue;
                }
                else if (cha.Type == CharacterType.Monster)
                {// 怪物
                    CharacterManager.Instance.AddCharacter(new Monster(cha));
                    continue;
                }
                else
                {// 其他玩家
                    CharacterManager.Instance.AddCharacter(new Character(cha));

                }
            }
        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse message)
        {
            Debug.LogFormat("OnMapCharacterLeave:Map:{0} CharacterId:{1}", User.Instance.CurrentMapData.ID, message.EntityId);

            if (User.Instance.CurrentCharacterInfo != null && message.EntityId != User.Instance.CurrentCharacterInfo.EntityId)
            {
                CharacterManager.Instance.RemoveCharacter(message.EntityId);
            }
            else
            {
                if (User.Instance.CurrentCharacterObject != null)
                {
                    User.Instance.CurrentCharacterObject.OnLeaveMap();
                }
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
                this._isLoadingDone = false;
                SceneManager.Instance.LoadScene(map.Resource);
                SoundManager.Instance.PlayMusic(map.Music);
                if (!MapSystemInitDone)
                {
                    Resloader.Instance.LoadAssetSync("Assets/AssetBundle/Prefab/Level/MapSystem.prefab").Instantiate();
                    MapSystemInitDone = true;
                }
            }
            else
                LogHelper.LogErrorFormat("EnterMap: Map {0} not existed", LogUser.MapService, mapId);
        }

        private void OnMapEntitySync(object sender, MapEntitySyncResponse message)
        {
            if (!this._isLoadingDone) { return; }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("MapEntityUpdateResponse : Entitis:{0}", message.EntitySyncs.Count);
            sb.AppendLine();
            foreach (var entity in message.EntitySyncs)
            {
                EntityManager.Instance.OnEntitySync(entity);
                sb.AppendFormat("[{0}] event:{1} entity:{2}", entity.Id, entity.Event, entity.Entity.String());
                sb.AppendLine();
            }
            LogHelper.Log(sb.ToString(), LogUser.MapService);
        }

        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entityData, int param)
        {
            if (!this._isLoadingDone) { return; }
            LogHelper.LogFormat("MapEntityUpdateRequest : ID:{0} POS:{1} DIR:{2} SPEED:{3} ", LogUser.MapService, entityData.Id, entityData.Position, entityData.Direction, entityData.Speed);
            NetMessage meg = new NetMessage();
            meg.Request = new NetMessageRequest();
            meg.Request.MapEntitySync = new MapEntitySyncRequest();

            meg.Request.MapEntitySync.EntitySync = new NEntitySync()
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
            meg.Request.MapTeleport = new MapTeleportRequest();
            meg.Request.MapTeleport.TeleporterId = id;
            NetClient.Instance.SendMessage(meg);
        }
    }
}
