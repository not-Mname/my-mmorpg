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

        private NDamageInfo _damageInfo;
        public NDamageInfo DamageInfo { get { return this._damageInfo; } }

        private float _cd;
        public float CD { get { return _cd; } }// 技能剩余冷却时间

        public bool IsCasting { get; set; }

        private float _castTime;
        private float _skillTime;
        private int _hit;

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
                if (distance > this.Define.CastRange)
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

        public void BeginCast(NDamageInfo damageInfo)
        {
            this.IsCasting = true;
            this._castTime = 0;
            this._skillTime = 0;
            this._cd = this.Define.CD;
            this._damageInfo = damageInfo;
            this.Owner.PlayAnim(this.Define.SkillAnim);
        }

        public void OnUpdate(float delta)
        {
            if (this.IsCasting)
            {
                this._skillTime += delta;
                if (this._skillTime > 0.5 && this._hit == 0)
                {
                    this.DoHit();
                }
            }

            this.UpdateCD(delta);
        }

        private void DoHit()
        {
            this._hit++;
            if (this._damageInfo != null)
            {
                var cha = CharacterManager.Instance.GetCharacter(_damageInfo.entityId);
                cha.DoDamage(this._damageInfo);
            }
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
