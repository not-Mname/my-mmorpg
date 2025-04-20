using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Network
{
    interface IPostResponser
    {
        void PostProcess(NetMessageResponse message);
        void Clear();
    }
}
