using Common;
using GameInterFace;
using GameServer.Managers.MBattle;
using Network;
using SkillBridge.Message;
using System;



namespace GameServer.Services.Battle
{
    class BattleSerevice : Singleton<BattleSerevice>, IDisposable, IInitializable
    {
       
        public void Init()
        {
            Log.Info("BattleSerevice Init...");

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<SkillCastRequest>(this.OnSkillCast);
        }

        private void OnSkillCast(NetConnection<NetSession> sender, SkillCastRequest message)
        {
            Log.InfoFormat("SendSkillCast: skillId {0}, casterId {1}, targetId {2}, position {3}", message.CastInfo.SkillId, message.CastInfo.CasterId, message.CastInfo.TargetId, message.CastInfo.Position);
            BattleManager.Instance.ProcessBattleMessage(sender, message);
        }

        public void Dispose()
        {

        }

    }
}
