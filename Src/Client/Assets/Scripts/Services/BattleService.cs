using Entities;
using Managers;
using Network;
using SkillBridge.Message;
using System;
using Utilities;

namespace Services
{
    class BattleService : Singleton<BattleService>, IDisposable
    {
        public BattleService()
        {
            MessageDistributer.Instance.Subscribe<SkillCastResponse>(OnSkillCast);
            MessageDistributer.Instance.Subscribe<SkillHitResponse>(OnSkillHit);

        }

        

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<SkillCastResponse>(OnSkillCast);
            MessageDistributer.Instance.Unsubscribe<SkillHitResponse>(OnSkillHit);
        }

        public void Init()
        {

        }

        public void SendSkillCast(int skillId, int casterId, int targetId, NVector3 currentPosition)
        {
            if (currentPosition == null)
            {
                currentPosition = new NVector3();
            }
            LogHelper.LogFormat("SendSkillCast: skillId {0}, casterId {1}, targetId {2}", LogUser.BattleService, skillId, casterId, targetId);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.skillCast = new SkillCastRequest();
            message.Request.skillCast.castInfo = new NSkillCastInfo();
            message.Request.skillCast.castInfo.skillId = skillId;
            message.Request.skillCast.castInfo.casterId = casterId;
            message.Request.skillCast.castInfo.targetId = targetId;
            message.Request.skillCast.castInfo.Position = currentPosition;
            NetClient.Instance.SendMessage(message);
        }

        public void OnSkillCast(object sender, SkillCastResponse response)
        {
            LogHelper.LogFormat("OnSkillCast: skillId {0}, casterId {1}, targetId {2}, position {3}", LogUser.BattleService, response.castInfo.skillId, response.castInfo.casterId, response.castInfo.targetId, response.castInfo.Position.String());
            if (response.Result == Result.Success)
            {
                BattleUnit caster = EntityManager.Instance.GetEntity(response.castInfo.casterId) as BattleUnit;
                if (caster != null)
                {
                    BattleUnit target = EntityManager.Instance.GetEntity(response.castInfo.targetId) as BattleUnit;
                    caster.CastSkill(response.castInfo.skillId, target, response.castInfo.Position, response.nDamageInfo);
                }
            }
            else
            {
                ChatManager.Instance.AddSystemMessage(response.Errormsg);
            }
        }

        private void OnSkillHit(object sender, SkillHitResponse message)
        {
            if(message.Result == Result.Success)
            {
                foreach(var hit in message.Hits)
                {
                    BattleUnit caster = EntityManager.Instance.GetEntity(hit.casterId) as BattleUnit;
                    if(caster != null)
                    {
                        caster.DoSkillHit(hit.skillId, hit.hitId, hit.Damages);
                    }
                }
            }
        }
    }
}