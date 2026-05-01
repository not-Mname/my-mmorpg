using Common;
using Common.Battle;
using Common.Data;
using Effect;
using Entities;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Utilities;

namespace Battle
{
    public class Skill
    {
        public NSkillInfo Info { get; set; }

        public BattleUnit Owner { get; set; }

        public SkillDefine Define { get; set; }

        private float _cd;
        public BattleUnit Target;

        public float CD { get { return _cd; } }// 技能剩余冷却时间

        public bool IsCasting { get; set; }
        public SkillStatus _status;

        private float _castTime;
        private float _skillTime;
        public int Hit;
        private Dictionary<int, List<NDamageInfo>> _hitMap = new Dictionary<int, List<NDamageInfo>>();
        private List<Bullet> _bullets = new List<Bullet>();
        private NVector3 _targetPosition;

        public Skill(NSkillInfo info, BattleUnit owner)
        {
            this.Info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.TID][info.Id];
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

        /// <summary>
        /// 技能释放的入口函数
        /// </summary>
        /// <param name="target"></param>
        public void BeginCast(BattleUnit target, NVector3 position)
        {
            this.IsCasting = true;
            this._castTime = 0;
            this._skillTime = 0;
            this.Hit = 0;
            this._cd = this.Define.CD;
            this.Target = target;
            this._targetPosition = position;
            this.Owner.PlayAnim(this.Define.SkillAnim);
            this._bullets.Clear();
            this._hitMap.Clear();

            if(this.Define.CastTarget == TargetType.Position)
            {
                this.Owner.FaceTo(_targetPosition.ToVector3Int());
            }else if(this.Define.CastTarget == TargetType.Target)
            {
                this.Owner.FaceTo(target.Position);
            }

            if (Define.CastTime > 0)
            {
                this._status = SkillStatus.Casting;
            }
            else
            {
                StartSkill();
            }
        }

        private void StartSkill()
        {
            this._status = SkillStatus.Running;
            if (!string.IsNullOrEmpty(this.Define.AOEEffect))
            {
                if(this.Define.CastTarget == TargetType.Position)
                {
                    this.Owner.PlayEffect(EffectType.Position, this.Define.AOEEffect,_targetPosition);

                }else if( this.Define.CastTarget == TargetType.Target)
                {
                    this.Owner.PlayEffect(EffectType.Position, this.Define.AOEEffect, Target);

                }else if( this.Define.CastTarget == TargetType.Self)
                {
                    this.Owner.PlayEffect(EffectType.Position, this.Define.AOEEffect, Owner);

                }
            }
        }

        private void CastBullet()
        {
            var bullet = new Bullet(this);
            this._bullets.Add(bullet);
            this.Owner.PlayEffect(EffectType.Bullet, this.Define.BulletResource, this.Target, bullet.Duration);
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
                UpdateSkill();
            }
        }

        private void UpdateSkill()
        {
            this._skillTime += Time.deltaTime;

            if (this.Define.Duration > 0)
            {
                // 持续型技能逻辑

                if (this._skillTime > this.Define.Interval * (this.Hit + 1))
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

                if (this.Hit < this.Define.HitTimes.Count)
                {
                    if (this._skillTime > this.Define.HitTimes[this.Hit])
                    {
                        // 达到预设的攻击时间点
                        DoHit();
                    }
                }
                else
                {
                    if (!this.Define.Bullet)
                    {
                        // 所有攻击次数已完成
                        this._status = SkillStatus.None;
                        Log.InfoFormat("Skill [{0}] done", this.Define.Name);
                    }
                }
            }

            if (this.Define.Bullet)
            {
                bool finish = true;
                foreach (var bullet in this._bullets)
                {
                    bullet.Update();
                    if (!bullet.Stoped)
                    {// 子弹还在飞行中
                        finish = false;
                    }
                }

                if (finish && this.Hit >= this.Define.HitTimes.Count)
                {
                    this._status = SkillStatus.None;
                    Log.InfoFormat("Bullet Skill [{0}] done", this.Define.Name);
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
                StartSkill();
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
            if (this.Define.Bullet)
            {
                this.CastBullet();
            }
            else
            {
                DoHitDamages(this.Hit);
            }
            this.Hit++;
        }

        internal void DoHit(NSkillHitInfo hit)
        {            
            if(hit.IsBullet || !this.Define.Bullet)
            {
                // 子弹命中 或者 非子弹技能 直接处理伤害 因为子弹技能要先释放子弹
                DoHit(hit.HitId, hit.Damages.ToList());
            }
        }

        internal void DoHit(int hitId, List<NDamageInfo> damages)
        {
            if (hitId > this.Hit)
            {
                // 如果消息来早了，缓存起来
                this._hitMap[hitId] = damages;
            }
            else
            {
                DoHitDamages(damages);
            }
        }

        public void DoHitDamages(int hitId)
        {
            if (this._hitMap.TryGetValue(hitId, out var damageList))
            {
                DoHitDamages(damageList);
            }
        }

        private void DoHitDamages(List<NDamageInfo> damages)
        {
            foreach (var damage in damages)
            {
                var unit = EntityManager.Instance.GetEntity(damage.EntityId) as BattleUnit;
                if (unit == null)
                {
                    continue;
                }
                unit.DoDamage(damage);
                if(this.Define.HitEffect != null)
                {
                    unit.PlayEffect(EffectType.Hit, this.Define.HitEffect, unit);
                }
            }
        }
        #endregion
    }
}
