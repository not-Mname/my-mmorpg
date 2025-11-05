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
        private float _cd;
        public float CD { get { return _cd; }}// 技能剩余冷却时间
        public bool IsCasting { get; set; }
        public int CastTime { get; private set; }

        public Skill(NSkillInfo info, BattleUnit owner)
        {
            this.Info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][info.Id];
            this._cd = 0;
        }

        public SkillResult CanCast(BattleUnit target)
        {
            if (this.Define.CastTarget == TargetType.Target && BattleManager.Instance.CurrentTarget == null)
            {
                if (target == null || target == this.Owner)
                {
                    return SkillResult.InvalidTarget;
                }

                int distance = (int)Vector3Int.Distance(this.Owner.Position, target.Position);
                if(distance > this.Define.CastRange)
                {
                    return SkillResult.OutOfRange;
                }
            }

            if (this.Define.CastTarget == TargetType.Position && BattleManager.Instance.CurrentPosition == null)
            {
                return SkillResult.InvalidTarget;
            }

            if (this.Owner.Attributes.MP < this.Define.MPCost)
            {
                return SkillResult.OutOfMp;
            }

            if (this._cd > 0)
            {
                return SkillResult.CoolDown;
            }

            return SkillResult.Ok;
        }

        public void Cast()
        {

        }

        public void BeginCast()
        {
            this.IsCasting = true;
            this.CastTime = 0;
            this._cd = this.Define.CD;
            this.Owner.PlayAnim(this.Define.SkillAnim);
        }

        public void OnUpdate(float delta)
        {
            if (this.IsCasting)
            {
            }

            this.UpdateCD(delta);
        }

        private void UpdateCD(float delta)
        {
            if (this.CD > 0)
            {
                this._cd -= delta;
            }
            else if (this.CD < 0)
            {
                this._cd = 0;
            }
        }
    }
}
