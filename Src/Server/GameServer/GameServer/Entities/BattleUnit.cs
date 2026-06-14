using Battle;
using Common.Battle;
using Common.Data;
using GameServer.GBattle;
using GameServer.Core;
using GameServer.Managers.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Entities
{
    class BattleUnit : Entity
    {
        /// <summary>
        /// 数据库id
        /// </summary>
        public int Id { get; set; }
        public string Name { get { return this.Info.Name; } }

        public bool IsDeath = false;

        public NCharacterInfo Info;
        public CharacterDefine Define;
        public SkillManager SkillManager;
        public BuffManager BuffManager;
        public EffectManager EffectManager;
        public Attributes Attributes;
        public BattleState BattleState;
        public CharacterState CharacterState;

        #region 初始化

        public BattleUnit(CharacterType type, int configId, int level, Vector3Int pos, Vector3Int dir) :
           base(pos, dir)
        {
            this.Define = DataManager.Instance.Characters[configId];
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Level = level;
            this.Info.ConfigId = configId;
            this.Info.Entity = this.EntityData;
            this.Info.EntityId = this.EntityId; 
            this.Info.Name = this.Define.Name;
            this.InitSkills();
            this.InitBuffs();

            this.Attributes = new Attributes();
            this.Attributes.Init(this.Define, this.GetEquips(), this.Info.Level, this.Info.Dynamic);
            this.Info.Dynamic = this.Attributes.Dynamic;
        }

        private void InitBuffs()
        {
            this.BuffManager = new BuffManager(this);
            this.EffectManager = new EffectManager(this);
        }

        private void InitSkills()
        {
            this.SkillManager = new SkillManager(this);
            this.Info.Skills.AddRange(this.SkillManager.Infos);
        }

        #endregion

        #region 战斗相关

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <param name="skillId">技能id</param>
        internal void CastSkill(BattleContext context, int skillId)
        {
            Skill skill = this.SkillManager.GetSkill(skillId);
            context.Result = skill.Cast(context);
            if (context.Result == SkillResult.Ok)
            {
                this.BattleState = BattleState.InBattle;
            }

            if(context.CastInfo == null)
            {
                if(context.Result == SkillResult.Ok)
                {
                    context.CastInfo = new NSkillCastInfo()
                    {
                        CasterId = this.EntityId,
                        TargetId = context.Target.EntityId,
                        SkillId = skill.Define.ID,
                        Position = new NVector3(),
                        Result = context.Result,
                    };
                    context.Battle.AddCastInfo(context.CastInfo);
                }
                
            }
            else
            {
                context.CastInfo.Result = context.Result;
                context.Battle.AddCastInfo(context.CastInfo);
            }
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage"></param>
        internal void DoDamage(NDamageInfo damage, BattleUnit source)
        {
            this.BattleState = BattleState.InBattle;
            this.Attributes.HP -= damage.Damage;
            if (this.Attributes.HP <= 0)
            {
                this.IsDeath = true;
                damage.WillDead = true;
            }
            this.OnDamage(damage, source);
        }

        protected virtual void OnDamage(NDamageInfo damage, BattleUnit source)
        {
            
        }
        #endregion

        public virtual List<EquipDefine> GetEquips()
        {
            return null;
        }

        public override void Update()
        {
            this.SkillManager.Update();
            this.BuffManager.Update();
        }

        public int Distance(BattleUnit target)
        {
            return (int)Vector3Int.Distance(this.Position, target.Position);
        }

        public int Distance(Vector3Int target)
        {
            return (int)Vector3Int.Distance(this.Position, target);
        }

        internal void AddBuff(BattleContext context, BuffDefine buffDefine)
        {
            this.BuffManager.AddBuff(context, buffDefine);
        }
    }
}
