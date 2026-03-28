using Common;
using Common.Battle;
using Common.Data;
using Common.Utils;
using GameServer.Battle;
using GameServer.Core;
using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace Battle
{
    /// <summary>
    /// 技能类，负责处理技能的所有逻辑，包括施放、冷却、伤害计算等
    /// </summary>
    class Skill
    {
        /// <summary>
        /// 技能的网络信息
        /// </summary>
        public NSkillInfo Info { get; set; }
        /// <summary>
        /// 技能拥有者(施法者)
        /// </summary>
        public BattleUnit Owner { get; set; }
        /// <summary>
        /// 技能配置数据
        /// </summary>
        public SkillDefine Define { get; set; }

        private float _cd;
        public float CD
        {
            get
            {
                return _cd;
            }
        }

        public SkillStatus Status { get; set; }

        /// <summary>
        /// 是否瞬发技能
        /// </summary>
        public bool Instant
        {
            get
            {
                if (this.Define.CastTime > 0)
                {
                    return false;
                }
                if (this.Define.Bullet)
                {
                    return false;
                }
                if (this.Define.Duration > 0)
                {
                    return false;
                }
                if (this.Define.HitTimes != null && this.Define.HitTimes.Count > 0)
                {
                    return false;
                }
                return true;
            }
        }

        private int _hit = 0;
        private float _castingTime = 0;
        private float _skillTime = 0;
        private BattleContext _context;
        private NSkillHitInfo _hitInfo;

        #region Update

        /// <summary>
        /// 每帧更新技能状态
        /// </summary>
        internal void Update()
        {
            // 更新冷却时间
            UpdateCD();

            // 根据技能状态调用不同的更新逻辑
            if (this.Status == SkillStatus.Casting)
            {
                this.UpdateCasting();
            }
            else if (this.Status == SkillStatus.Running)
            {
                this.UpdateSkill();
            }
        }

        /// <summary>
        /// 更新技能运行状态
        /// </summary>
        private void UpdateSkill()
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
                    this.Status = SkillStatus.None;
                    Log.InfoFormat("Skill [{0}] done", this.Define.Name);
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
                    this.Status = SkillStatus.None;
                    Log.InfoFormat("Skill [{0}] done", this.Define.Name);
                }
            }
        }

        /// <summary>
        /// 更新吟唱状态
        /// </summary>
        private void UpdateCasting()
        {
            if (this._castingTime < this.Define.CastTime)
            {
                // 吟唱时间未结束，继续累加
                _castingTime += Time.deltaTime;
            }
            else
            {
                // 吟唱结束，切换到运行状态
                _castingTime = 0;
                this.Status = SkillStatus.Running;
                Log.InfoFormat("Skill {0} cast done, Name {1}", this.Info.Id, this.Define.Name);
            }
        }

        /// <summary>
        /// 更新冷却时间
        /// </summary>
        private void UpdateCD()
        {
            if (this.CD > 0)
            {
                // 冷却时间递减
                this._cd -= Time.deltaTime;
            }
            else if (this.CD < 0)
            {
                // 确保冷却时间不会为负值
                this._cd = 0;
            }
        }
        #endregion

        #region Cast
        /// <summary>
        /// 检查技能是否可以施放
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <returns>返回技能施放结果</returns>
        public SkillResult CanCast(BattleContext context)
        {
            // 技能正在施放中
            if (this.Status != SkillStatus.None)
            {
                return SkillResult.Casting;
            }

            // 检查目标类型为单体目标的情况
            if (this.Define.CastTarget == TargetType.Target)
            {
                // 目标无效检查
                if (context.Target == null || context.Target == this.Owner)
                {
                    return SkillResult.InvalidTarget;
                }
                // 距离检查
                int distance = this.Owner.Distance(context.Target);
                if (distance > this.Define.CastRange)
                {
                    return SkillResult.OutOfRange;
                }
            }

            // 检查目标类型为位置的情况
            if (this.Define.CastTarget == TargetType.Position)
            {
                if (context.CastInfo.Position == null)
                {
                    return SkillResult.InvalidTarget;
                }
                if (this.Owner.Distance(context.Position) > this.Define.CastRange)
                {
                    return SkillResult.OutOfRange;
                }
            }

            // MP不足检查
            if (this.Owner.Attribute.MP < this.Define.MPCost)
            {
                return SkillResult.OutOfMp;
            }

            // 冷却检查
            if (this.CD > 0)
            {
                return SkillResult.CoolDown;
            }

            return SkillResult.Ok;
        }

        public SkillResult Cast(BattleContext context)
        {
            SkillResult result = CanCast(context);

            this._castingTime = 0;
            this._skillTime = 0;
            this._cd = this.Define.CD;
            this._context = context;
            this._hit = 0;

            if (result == SkillResult.Ok)
            {
                if (this.Instant)
                {
                    this.DoHit();
                }
                else
                {
                    if (this.Define.CastTime > 0)
                    {
                        this.Status = SkillStatus.Casting;
                    }
                    else
                    {
                        this.Status = SkillStatus.Running;
                    }
                }
            }
            Log.InfoFormat("Skill [{0}] cast result {1} status {2}", this.Define.Name, result, this.Status);
            return result;
        }

        private void CastBullet()
        {
            Log.InfoFormat("skill [{1}] cast bullet", this.Define.Name, this.Define.BulletResource);
        }
        #endregion

        #region Hit
        private void DoHit()
        {
            this.InitHitInfo();
            Log.InfoFormat("Skill [{0}] hit {1}", this.Define.Name, _hit);
            this._hit++;

            if (this.Define.Bullet)
            {
                this.CastBullet();
                return;
            }

            if (this.Define.AOERange > 0)
            {
                this.HitRange();
                return;
            }

            if (this.Define.CastTarget == TargetType.Target)
            {
                this.HitTarget(this._context.Target);
            }
        }

        /// <summary>
        /// 对单个目标造成伤害
        /// </summary>
        /// <param name="target">目标单位</param>
        private void HitTarget(BattleUnit target)
        {
            // 目标验证检查
            if (this.Define.CastTarget == TargetType.Self && target != _context.Caster)
            {
                return; // 仅对自身施放的技能，目标不是施法者则跳过
            }
            else if (this.Define.CastTarget != TargetType.Self && target == _context.Caster)
            {
                return; // 非对自身施放的技能，目标是施法者则跳过
            }

            // 计算伤害并应用
            NDamageInfo damage = this.CaculateDamage(this._context.Caster, target);
            Log.InfoFormat("Skill [{0}] damage {1} hit target {2} hit caster{3}", this.Define.Name, damage.Damage, target.Define.Name, _context.Caster.Define.Name);
            target.DoDamage(damage);
            this._hitInfo.Damages.Add(damage);
        }
        private void HitRange()
        {
            Vector3Int pos;
            if (this.Define.CastTarget == TargetType.Target)
            {
                pos = this._context.Target.Position;
            }
            else if (this.Define.CastTarget == TargetType.Position)
            {
                pos = this._context.CastInfo.Position;
            }
            else
            {
                pos = this._context.Caster.Position;
            }

            List<BattleUnit> units = this._context.Battle.FindUnitsInRange(pos, this.Define.AOERange);
            foreach (var unit in units)
            {
                this.HitTarget(unit);
            }
        }

        /// <summary>
        /// 施放子弹类型技能
        /// </summary>


        private void InitHitInfo()
        {
            this._hitInfo = new NSkillHitInfo();
            _hitInfo.casterId = this._context.Caster.entityId;
            _hitInfo.skillId = this.Info.Id;
            _hitInfo.hitId = this._hit;
            _context.Battle.AddHitInfo(_hitInfo);
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="info">技能网络信息</param>
        /// <param name="owner">技能拥有者</param>
        public Skill(NSkillInfo info, BattleUnit owner)
        {
            this.Info = info;
            this.Owner = owner;
            // 根据职业和技能ID从数据管理器获取技能配置
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][info.Id];
        }


        /// <summary>
        /// 战斗计算
        /// 公式:
        /// 物理伤害 = 物理攻击或技能原始伤害 * (1 - 物理防御 / (物理防御 + 100))
        /// 魔法伤害 = 法术攻击或技能原始伤害 * (1 - 魔法防御 / (魔法防御+100))
        /// 暴击伤害 = 固定两倍伤害
        /// 注:伤害值最小值为1.当伤害值小于1的时候取1
        /// 注:最终伤害值在最终取舍时随机浮动5%.即：最终伤害 = range（最终伤害值 * (1 - 5%)  最终伤害值 * (1 + 5%)） 
        /// </summary>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>伤害信息</returns>
        private NDamageInfo CaculateDamage(BattleUnit caster, BattleUnit target)
        {
            // 计算物理攻击和法术攻击
            float ad = caster.Attribute.AD + this.Define.AD * this.Define.ADFator;
            float ap = caster.Attribute.AP + this.Define.AP * this.Define.APFator;

            // 计算物理伤害和法术伤害
            float ad_damage = ad * ((1 - target.Attribute.DEF) / (target.Attribute.DEF + 100));
            float ap_damage = ap * ((1 - target.Attribute.MDEF) / (target.Attribute.MDEF + 100));

            // 计算总伤害
            float final_damage = ad_damage + ap_damage;
            
            // 暴击判定
            bool isCritical = IsCritical(caster.Attribute.CRI);
            if (isCritical)
            {
                final_damage *= 2;
            }
            
            // 伤害浮动(±5%)
            final_damage += final_damage * ((float)MathUtil.Random.NextDouble() * 0.1f - 0.05f);
            
            // 确保最小伤害为1
            return new NDamageInfo() { 
                Damage = Math.Max(1, (int)final_damage), 
                entityId = target.entityId, 
                Critical = isCritical 
            };
        }

        /// <summary>
        /// 判断是否暴击
        /// </summary>
        /// <param name="critical">暴击率(0-1之间的小数)</param>
        /// <returns>是否暴击</returns>
        private bool IsCritical(float critical)
        {
            return MathUtil.Random.NextDouble() < critical;
        }



    }
}
