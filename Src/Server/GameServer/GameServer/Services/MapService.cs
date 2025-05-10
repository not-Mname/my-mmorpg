using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>, IDisposable
    {

        public MapService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport);
        }

        public void Dispose()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
        }

        public void Init()
        {
            MapManager.Instance.Init();
        }

        internal void SendEntityUpdate(NetConnection<NetSession> connection, NEntitySync entitySync)
        {
            connection.Session.Response.mapEntitySync = new MapEntitySyncResponse();
            connection.Session.Response.mapEntitySync.entitySyncs.Add(entitySync);
            connection.SendResponse();
        }

        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapEntitySync : CharacterID {0} EntityID {1} Event {2} Entity {3}", character.Id, message.entitySync.Id, message.entitySync.Event, message.entitySync.Entity.String());
            MapManager.Instance[character.Info.mapId].UpdateEntity(message.entitySync);
        }

        private void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapTeleport : CharacterID {0} MapID {1}", character.Id, message.teleporterId);
            if (!DataManager.Instance.Teleporters.ContainsKey(message.teleporterId))
            {
                Log.WarningFormat("OnMapTeleport : TeleporterID {0} not found", message.teleporterId);
                return;
            }

            TeleporterDefine td = DataManager.Instance.Teleporters[message.teleporterId];

            if (td.LinkTo == 0 || !DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))
            {
                Log.WarningFormat("OnMapTeleport : TeleporterID {0} LinkTo {1} not found", message.teleporterId, td.LinkTo);
                return;
            }

            TeleporterDefine linkTo = DataManager.Instance.Teleporters[td.LinkTo];

            MapManager.Instance[character.Data.MapID].CharacterLeave(character);

            character.Position = linkTo.Position;
            character.Direction = linkTo.Direction;
            MapManager.Instance[linkTo.MapID].CharacterEnter(sender, character);
        }


    }
}