using GameServer.Entities;
using GameServer.Models.Data;
using GameServer.Network;
using GameServer.Services.Entities;
using SkillBridge.Message;

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
            if(response != null)
            {
                if(PostResponser != null)
                {
                    this.PostResponser.PostProcess(response.Response);
                }
                byte[] bytes = PackageHandler.PackMessage(response);
                response = null;
                return bytes;
            }
            return null;
        }

        NetMessage response;

        public NetMessageResponse Response
        {
            get
            {
                if(response == null)
                    response = new NetMessage();
                if(response.Response == null)
                    response.Response = new NetMessageResponse();
                return response.Response;
            }
        }

    }
}
