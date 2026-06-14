using Common.Battle;
using Common.Data;
using Common.Utils;
using GameServer.GBattle;
using GameServer.Entities;
using SkillBridge.Message;
using System;

namespace Battle
{
    internal class Buff
    {
        public int BuffId;
        public bool Stopped { get; private set; }
        private BattleUnit _owner;
        private BuffDefine _buffDefine;
        private BattleContext _context;
        private float _time;
        private int _hit;

        public Buff(int buffId, BattleUnit owner, BuffDefine buffDefine, BattleContext context)
        {
            BuffId = buffId;
            _owner = owner;
            _buffDefine = buffDefine;
            _context = context;

            this.OnAdd();
        }

        internal void Update()
        {
            if (this.Stopped)
            {
                return;
            }
            _time += Time.DeltaTime;

            if(this._buffDefine.Interval > 0)
            {//如果有间隔时间
                if(_time > this._buffDefine.Interval * (this._hit + 1))
                {
                    this.DoBuffDamage();
                }
            }

            if(_time > this._buffDefine.Duration)
            {
                this.OnRemove();
            }
        }

        private void OnRemove()
        {
            RemoveAttribute();
            this.Stopped = true;
            if(this._buffDefine.Effect != BuffEffect.None)
            {
                this._owner.EffectManager.RemoveBuffEffect(this._buffDefine.Effect);
            }
            NBuffInfo buff = new NBuffInfo()
            {
                BuffId = this.BuffId,
                BuffType = this._buffDefine.ID,
                CasterId = this._context.Caster.EntityId,
                OwnerId = this._owner.EntityId,
                Action = BuffAction.Remove,
            };
            this._context.Battle.AddBuffAction(buff);
        }

        private void RemoveAttribute()
        {//todo 这里先只写一种情况，后续再扩展
            if (this._buffDefine.DEFRatio != 0)
            {
                this._owner.Attributes.Buff.DEF -= this._owner.Attributes.Basic.DEF * this._buffDefine.DEFRatio;
            }
            this._owner.Attributes.InitFinalAttribute();
        }

        private void DoBuffDamage()
        {
            this._hit++;
            NDamageInfo damage = CaculateDamage(this._context.Caster);
            this._owner.DoDamage(damage, this._context.Caster);

            NBuffInfo buff = new NBuffInfo()
            {
                BuffId = this.BuffId,
                BuffType = this._buffDefine.ID,
                CasterId = this._context.Caster.EntityId,
                OwnerId = this._owner.EntityId,
                Action = BuffAction.Hit,
                Damage = damage,
            };
            this._context.Battle.AddBuffAction(buff);
        }

        private NDamageInfo CaculateDamage(BattleUnit caster)
        {
            // 计算物理攻击和法术攻击
            float ad = _buffDefine.AD + caster.Attributes.AD * this._buffDefine.ADFator;
            float ap = _buffDefine.AP + caster.Attributes.AP * this._buffDefine.APFator;

            // 计算物理伤害和法术伤害
            float ad_damage = ad * ((1 - _owner.Attributes.DEF) / (_owner.Attributes.DEF + 100));
            float ap_damage = ap * ((1 - _owner.Attributes.MDEF) / (_owner.Attributes.MDEF + 100));

            // 计算总伤害
            float final_damage = ad_damage + ap_damage;

            // 确保最小伤害为1
            return new NDamageInfo()
            {
                Damage = Math.Max(1, (int)final_damage),
                EntityId = _owner.EntityId,
            };
        }

        private void OnAdd()
        {//todo 这里先只写一种情况，后续再扩展
            if (this._buffDefine.Effect != BuffEffect.None)
            {
                this._owner.EffectManager.AddBuffEffect(this._buffDefine.Effect);
            }

            AddAttribute();
            NBuffInfo buff = new NBuffInfo()
            {
                BuffId = this.BuffId,
                BuffType = this._buffDefine.ID,
                CasterId = this._context.Caster.EntityId,
                OwnerId = this._owner.EntityId,
                Action = BuffAction.Add,
            };
            this._context.Battle.AddBuffAction(buff);
        }

        private void AddAttribute()
        {
            if (this._buffDefine.DEFRatio != 0)
            {
                this._owner.Attributes.Buff.DEF += this._owner.Attributes.Basic.DEF * this._buffDefine.DEFRatio;    
            }

            this._owner.Attributes.InitFinalAttribute();
        }
    }
}
