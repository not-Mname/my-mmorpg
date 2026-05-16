using Common;
using GameServer.Entities;
using GameServer.Managers.Entities;
using Network;
using SkillBridge.Message;
using System;


namespace GameServer.Managers.MBattle
{
    class BattleManager : Singleton<BattleManager>
    {
        public void ProcessBattleMessage(NetConnection<NetSession> sender, SkillCastRequest message)
        {
            Log.InfoFormat("SendSkillCast: skillId {0}, casterId {1}, targetId {2}, position {3}", message.CastInfo.SkillId, message.CastInfo.CasterId, message.CastInfo.TargetId, message.CastInfo.Position.String());
            Character cha = sender.Session.Character;
            var battle = MapManager.Instance[cha.Info.MapId].Battle;

            battle.ProcessBattleMessage(sender, message);
        }
    }
}
