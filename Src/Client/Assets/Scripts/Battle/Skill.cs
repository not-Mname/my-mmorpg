using Common.Battle;
using Common.Data;
using Entities;
using Managers;
using SkillBridge.Message;
using System;
using UnityEngine;

namespace Battle
{
    public class Skill
    {
        public NSkillInfo Info { get; set; }
        public BattleUnit Owner { get; set; }
        public SkillDefine Define { get; set; }
        public float CD { get; set; }

        public Skill(NSkillInfo info, BattleUnit owner)
        {
            this.Info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][info.Id];
        }

        public SkillResult CanCast()
        {
            if(this.Define.CastTarget == TargetType.Target && BattleManager.Instance.Target == null)
            {
                return SkillResult.InvalidTarget;
            }

            if (this.Define.CastTarget == TargetType.Position && BattleManager.Instance.Position == Vector3.negativeInfinity)
            {
                return SkillResult.InvalidTarget;
            }

            if(this.Owner.Attributes.MP < this.Define.MPCost)
            {
                return SkillResult.OutOfMp;
            }

            if(this.CD > 0)
            {
                return SkillResult.CoolDown;
            }

            return SkillResult.OK;
        }

        public void Cast()
        {
            
        }
    }
}
