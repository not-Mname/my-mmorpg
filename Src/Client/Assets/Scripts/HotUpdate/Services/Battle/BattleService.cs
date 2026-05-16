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
            MessageDistributer.Instance.Subscribe<BuffResponse>(OnBuff);
        }



        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<SkillCastResponse>(OnSkillCast);
            MessageDistributer.Instance.Unsubscribe<SkillHitResponse>(OnSkillHit);
            MessageDistributer.Instance.Unsubscribe<BuffResponse>(OnBuff);
        }

        public void Init()
        {

        }
        private void OnBuff(object sender, BuffResponse message)
        {
            foreach (var buff in message.Buffs)
            {
                BattleUnit unit = EntityManager.Instance.GetEntity(buff.OwnerId) as BattleUnit;
                if (unit != null)
                {
                    unit.DoBuffAction(buff);
                }
            }
        }

        public void SendSkillCast(int skillId, int casterId, int targetId, NVector3 currentPosition)
        {
            if (currentPosition == null)
            {
                currentPosition = new NVector3();
            }
            LogHelper.LogFormat("SendSkillCast: skillId {0}, CasterId {1}, TargetId {2}", LogUser.BattleService, skillId, casterId, targetId);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.SkillCast = new SkillCastRequest();
            message.Request.SkillCast.CastInfo = new NSkillCastInfo();
            message.Request.SkillCast.CastInfo.SkillId = skillId;
            message.Request.SkillCast.CastInfo.CasterId = casterId;
            message.Request.SkillCast.CastInfo.TargetId = targetId;
            message.Request.SkillCast.CastInfo.Position = currentPosition;
            NetClient.Instance.SendMessage(message);
        }

        public void OnSkillCast(object sender, SkillCastResponse response)
        {
            if (response.Result == Result.Success)
            {
                foreach (var CastInfo in response.CastInfos)
                {
                    LogHelper.LogFormat("OnSkillCast: skillId {0}, CasterId {1}, TargetId {2}, position {3}", LogUser.BattleService,CastInfo.SkillId, CastInfo.CasterId, CastInfo.TargetId, CastInfo.Position.String());
                    BattleUnit caster = EntityManager.Instance.GetEntity(CastInfo.CasterId) as BattleUnit;
                    if (caster != null)
                    {
                        BattleUnit target = EntityManager.Instance.GetEntity(CastInfo.TargetId) as BattleUnit;
                        caster.CastSkill(CastInfo.SkillId, target, CastInfo.Position);
                    }
                }
                
            }
            else
            {
                LogHelper.Log($"OnSkillCast: skill cast failed : {response.Errormsg}");
            }
        }

        private void OnSkillHit(object sender, SkillHitResponse message)
        {
            if(message.Result == Result.Success)
            {
                foreach(var hit in message.Hits)
                {
                    BattleUnit caster = EntityManager.Instance.GetEntity(hit.CasterId) as BattleUnit;
                    if(caster != null)
                    {
                        caster.DoSkillHit(hit);
                    }
                }
            }
        }

    }
}