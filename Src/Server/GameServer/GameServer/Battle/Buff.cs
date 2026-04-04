using Common.Battle;
using Common.Data;
using Common.Utils;
using GameServer.Battle;
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
            _time += Time.deltaTime;

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
                buffId = this.BuffId,
                buffType = this._buffDefine.ID,
                casterId = this._context.Caster.entityId,
                ownerId = this._owner.entityId,
                Action = BuffAction.Remove,
            };
            this._context.Battle.AddBuffAction(buff);
        }

        private void RemoveAttribute()
        {//todo 这里先只写一种情况，后续再扩展
            if (this._buffDefine.DEFRatio != 0)
            {
                this._owner.Attribute.Buff.DEF -= this._owner.Attribute.Basic.DEF * this._buffDefine.DEFRatio;
            }
            this._owner.Attribute.InitFinalAttribute();
        }

        private void DoBuffDamage()
        {
            this._hit++;
            NDamageInfo damage = CaculateDamage(this._context.Caster);
            this._owner.DoDamage(damage);

            NBuffInfo buff = new NBuffInfo()
            {
                buffId = this.BuffId,
                buffType = this._buffDefine.ID,
                casterId = this._context.Caster.entityId,
                ownerId = this._owner.entityId,
                Action = BuffAction.Hit,
                Damage = damage,
            };
        }

        private NDamageInfo CaculateDamage(BattleUnit caster)
        {
            // 计算物理攻击和法术攻击
            float ad = _buffDefine.AD + caster.Attribute.AD * this._buffDefine.ADFator;
            float ap = _buffDefine.AP + caster.Attribute.AP * this._buffDefine.APFator;

            // 计算物理伤害和法术伤害
            float ad_damage = ad * ((1 - _owner.Attribute.DEF) / (_owner.Attribute.DEF + 100));
            float ap_damage = ap * ((1 - _owner.Attribute.MDEF) / (_owner.Attribute.MDEF + 100));

            // 计算总伤害
            float final_damage = ad_damage + ap_damage;

            // 确保最小伤害为1
            return new NDamageInfo()
            {
                Damage = Math.Max(1, (int)final_damage),
                entityId = _owner.entityId,
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
                buffId = this.BuffId,
                buffType = this._buffDefine.ID,
                casterId = this._context.Caster.entityId,
                ownerId = this._owner.entityId,
                Action = BuffAction.Add,
            };
            this._context.Battle.AddBuffAction(buff);
        }

        private void AddAttribute()
        {
            if (this._buffDefine.DEFRatio != 0)
            {
                this._owner.Attribute.Buff.DEF += this._owner.Attribute.Basic.DEF * this._buffDefine.DEFRatio;    
            }

            this._owner.Attribute.InitFinalAttribute();
        }
    }
}
