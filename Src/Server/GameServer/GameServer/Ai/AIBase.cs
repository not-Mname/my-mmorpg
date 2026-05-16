using Battle;
using Common.Battle;
using GameServer.GBattle;
using GameServer.Entities;
using SkillBridge.Message;
using System;

namespace GameServer.AI
{
    internal class AIBase
    {
        protected Monster _owner;
        protected BattleUnit _target;
        public Skill NormalSkill;

        public AIBase(Monster owner)
        {
            this._owner = owner;
            this.NormalSkill = owner.SkillManager.NormalSkill;
        }


        internal void OnDamage(NDamageInfo damage, BattleUnit source)
        {
            this._target = source;
        }

        internal void ClearTarget(BattleUnit target)
        {
            if (this._target == target)
            {
                this._target = null;
                this._owner.BattleState = BattleState.Idle;
                this._owner.StopMove();
            }
        }

        internal void Update()
        {
            if (_owner.BattleState == BattleState.InBattle)
            {
                UpdateBattle();
            }
        }

        private void UpdateBattle()
        {
            if (this._target == null)
            {
                this._owner.BattleState = BattleState.Idle;
                return;
            }

            if (!TryCastSkill())
            {// 技能打不到
                if (!TryCastNormal())
                {// 普通攻击打不到
                    FollowTarget();
                }
            }
        }

        private void FollowTarget()
        {
            int distance = _owner.Distance(_target);
            if(distance > this.NormalSkill.Define.CastRange - 50)
            {
                this._owner.MoveTo(_target.Position);
            }
            else
            {
                this._owner.StopMove();
            }
        }

        private bool TryCastNormal()
        {
            if (this._target != null)
            {
                var context = new BattleContext(_owner.Map.Battle)
                {
                    Target = this._target,
                    Caster = _owner,
                };
                var castRes = NormalSkill.CanCast(context);
                if (castRes == SkillResult.Ok)
                {
                    _owner.CastSkill(context, NormalSkill.Define.ID);
                }
                return castRes == SkillResult.Ok;
            }
            
            return false;
        }

        private bool TryCastSkill()
        {
            if (this._target == null)
            {
                return false;
            }

            var context = new BattleContext(_owner.Map.Battle)
            {
                Target = this._target,
                Caster = _owner,
            };
            Skill skill = _owner.FindSkill(context, SkillType.Skill);
            if (skill != null)
            {

                _owner.CastSkill(context, skill.Define.ID);
            }
            return skill != null;
        }
    }
}
