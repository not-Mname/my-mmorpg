using Battle;
using Common.Battle;
using Common.Data;
using GameServer.Battle;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Entities
{
    class BattleUnit : Entity
    {

        public int Id { get; set; }
        public string Name { get { return this.Info.Name; } }

        public bool IsDeath = false;

        public NCharacterInfo Info;
        public CharacterDefine Define;
        public SkillManager SkillManager;
        public Attributes Attribute;

        public BattleUnit(CharacterType type, int configId, int level, Vector3Int pos, Vector3Int dir) :
           base(pos, dir)
        {
            this.Define = DataManager.Instance.Characters[configId];
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Level = level;
            this.Info.ConfigId = configId;
            this.Info.Entity = this.EntityData;
            this.Info.EntityId = this.entityId; 
            this.Info.Name = this.Define.Name;
            this.InitSkills();

            this.Attribute = new Attributes();
            this.Attribute.Init(this.Define, this.GetEquips(), this.Info.Level, this.Info.Dynamic);
            this.Info.Dynamic = this.Attribute.Dynamic;
        }

        public virtual List<EquipDefine> GetEquips()
        {
            return null;
        }

        private void InitSkills()
        {
            this.SkillManager = new SkillManager(this);
            this.Info.Skills.AddRange(this.SkillManager.Infos);
        }

        internal void CastSkill(BattleContext context, int skillId)
        {
            Skill skill = this.SkillManager.GetSkill(skillId);
            context.Result = skill.Cast(context);
        }


        internal void DoDamage(NDamageInfo damage)
        {
            this.Attribute.HP -= damage.Damage;
            if (this.Attribute.HP <= 0)
            {
                this.IsDeath = true;
                damage.WillDead = true;
            }
        }

        public override void Update()
        {
            this.SkillManager.Update();
        }
    }
}
