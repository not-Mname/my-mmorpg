using GameServer.Entities;
using GameServer.Models.Data;
using GameServer.Network;
using GameServer.Services.Entities;
using SkillBridge.Message;
using System;

namespace Network
{
    class NetSession : INetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }
        public IPostResponser PostResponser { get; set; }
        internal void DisConnected()
        {
            this.PostResponser = null;
            if (Character != null)
            {
                UserService.Instance.CharacterLeave(Character);
            }
        }

        public byte[] GetResponse()
        {
            if (response != null && response.Responses.Count > 0)
            {
                if (PostResponser != null)
                {
                    this.PostResponser.PostProcess(response);
                }
                byte[] bytes = PackageHandler.PackMessage(response);
                response = null;
                return bytes;
            }
            return null;
        }

        NetMessage response;

        public void AddResponse(NetMessageResponse item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.PayloadCase == NetMessageResponse.PayloadOneofCase.None)
            {
                throw new ArgumentException(
                    "Response envelope has no payload",
                    nameof(item));
            }

            if (response == null)
            {
                response = new NetMessage();
            }

            response.Responses.Add(item);
        }
    }
}
