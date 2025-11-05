using Common;
using Network;
using System.Collections.Generic;


namespace GameServer.Managers
{
    class SessionManager : Singleton<SessionManager>
    {
        //<characterId, session>
        public Dictionary<int, NetConnection<NetSession>> sessions = new Dictionary<int, NetConnection<NetSession>>();

        public void AddSession(int characterId, NetConnection<NetSession> session)
        {
            sessions[characterId] = session;
        }
        public void RemoveSession(int characterId)
        {
            sessions.Remove(characterId);
        }
        public NetConnection<NetSession> GetSession(int characterId)
        {
            NetConnection<NetSession> session;
            sessions.TryGetValue(characterId, out session);
            return session;
        }
    }
}
