using Common;
using Common.Battle;
using Common.Data;
using Entities;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

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
        public SkillStatus _status;

        private float _castTime;
        private float _skillTime;
        private int _hit;
        private Dictionary<int, List<NDamageInfo>> _hitMap = new Dictionary<int, List<NDamageInfo>>();

        public Skill(NSkillInfo info, BattleUnit owner)
        {
            this.Info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][info.Id];
            this._cd = 0;
        }
        #region cast

        public SkillResult CanCast(BattleUnit target)
        {
            if (this.Define.CastTarget == TargetType.Target && BattleManager.Instance.CurrentTarget == null)
            {
                if (target == null || target == this.Owner)
                {
                    return SkillResult.InvalidTarget;
                }

                int distance = Owner.Distance(target);
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
            this._hit = 0;
            this._cd = this.Define.CD;
            this._damageInfo = damageInfo;
            this.Owner.PlayAnim(this.Define.SkillAnim);

            if (Define.CastTime > 0)
            {
                this._status = SkillStatus.Casting;
            }
            else
            {
                this._status = SkillStatus.Running;
            }
        }

#endregion

        #region Update

        public void OnUpdate(float delta)
        {
            this.UpdateCD(delta);

            if (this._status == SkillStatus.Casting)
            {
                UpdateCasting();
            }
            else if (this._status == SkillStatus.Running)
            {
                UpdateRunning();
            }
        }

        private void UpdateRunning()
        {
            this._skillTime += Time.deltaTime;

            if (this.Define.Duration > 0)
            {
                // 持续型技能逻辑

                if (this._skillTime > this.Define.Interval * (this._hit + 1))
                {
                    // 达到攻击间隔时间，执行命中
                    DoHit();
                }

                if (this._skillTime > this.Define.Duration)
                {
                    // 技能持续时间结束
                    this._status = SkillStatus.None;
                    this.IsCasting = false;
                    LogHelper.Log($"Skill [{this.Define.Name}] done", LogUser.Battle);
                }
            }
            else if (this.Define.HitTimes != null && this.Define.HitTimes.Count > 0)
            {
                // 单次攻击或多次攻击技能逻辑

                if (this._hit < this.Define.HitTimes.Count)
                {
                    if (this._skillTime > this.Define.HitTimes[this._hit])
                    {
                        // 达到预设的攻击时间点
                        DoHit();
                    }
                }
                else
                {
                    // 所有攻击次数已完成
                    this._status = SkillStatus.None;
                    this.IsCasting = false;
                    LogHelper.Log($"Skill [{this.Define.Name}] done", LogUser.Battle);
                }
            }
        }

        private void UpdateCasting()
        {
            if(this._castTime < this.Define.CastTime)
            {
                _castTime += Time.deltaTime;
            }
            else
            {
                _castTime = 0;
                _status = SkillStatus.Running;
                LogHelper.Log($"Skill [{this.Define.ID}] [{this.Define.Name}] Casting Finished", LogUser.Battle);
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
        #endregion

        #region Hit
        private void DoHit()
        {
            if (this._hitMap.TryGetValue(this._hit, out var damageList))
            {
                DoHitDamages(damageList);
            }
            this._hit++;
        }

        internal void DoHit(int hitId, List<NDamageInfo> damages)
        {
            if (hitId > this._hit)//todo 疑似bug 这里先修改掉 原逻辑if(hitId <= this._hit)
            {
                // 如果消息来早了，缓存起来
                this._hitMap[hitId] = damages;
            }
            else
            {
                DoHitDamages(damages);
            }
        }

        private void DoHitDamages(List<NDamageInfo> damages)
        {
            foreach (var damage in damages)
            {
                var unit = EntityManager.Instance.GetEntity(damage.entityId) as BattleUnit;
                if (unit == null)
                {
                    continue;
                }
                unit.DoDamage(damage);
            }
        }
        #endregion
    }
}
