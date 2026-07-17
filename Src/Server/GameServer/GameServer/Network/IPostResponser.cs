using SkillBridge.Message;

namespace GameServer.Network
{
    interface IPostResponser
    {
        void PostProcess(NetMessage message);
        void Clear();
    }
}
