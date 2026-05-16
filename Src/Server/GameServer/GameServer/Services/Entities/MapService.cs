using Common;
using Common.Data;
using GameInterFace;
using GameServer.Entities;
using GameServer.Managers.Data;
using GameServer.Managers.Entities;
using Network;
using SkillBridge.Message;
using System;

namespace GameServer.Services.Entity
{
    class MapService : Singleton<MapService>, IDisposable, IInitializable
    {

        public MapService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport);
        }

        public void Dispose()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<MapTeleportRequest>(this.OnMapTeleport);
        }

        public void Init()
        {
            Log.Info("MapService Init...");
            MapManager.Instance.Init();
        }

        internal void SendEntityUpdate(NetConnection<NetSession> connection, NEntitySync EntitySync)
        {
            connection.Session.Response.MapEntitySync = new MapEntitySyncResponse();
            connection.Session.Response.MapEntitySync.EntitySyncs.Add(EntitySync);
            connection.SendResponse();
        }

        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest message)
        {
            Character character = sender.Session.Character;
            MapManager.Instance[character.Info.MapId].UpdateEntity(message.EntitySync);
        }

        private void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapTeleport : CharacterID {0} MapId {1}", character.Id, message.TeleporterId);
            if (!DataManager.Instance.Teleporters.ContainsKey(message.TeleporterId))
            {
                Log.WarningFormat("OnMapTeleport : TeleporterId {0} not found", message.TeleporterId);
                return;
            }

            TeleporterDefine td = DataManager.Instance.Teleporters[message.TeleporterId];

            if (td.LinkTo == 0 || !DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))
            {
                Log.WarningFormat("OnMapTeleport : TeleporterId {0} LinkTo {1} not found", message.TeleporterId, td.LinkTo);
                return;
            }

            TeleporterDefine linkTo = DataManager.Instance.Teleporters[td.LinkTo];

            MapManager.Instance[character.Info.MapId].CharacterLeave(character);

            character.Position = linkTo.Position;
            character.Direction = linkTo.Direction;
            MapManager.Instance[linkTo.MapID].CharacterEnter(sender, character);
        }


    }
}