using Battle;
using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;

namespace GameServer.Entities
{
    public class BattleUnit : Entity
    {

        public int Id { get; set; }
        public string Name { get { return this.Info.Name; } }
        public NCharacterInfo Info;
        public CharacterDefine Define;
        public SkillManager skillManager;

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
        }

        private void InitSkills()
        {
            this.skillManager = new SkillManager(this);
            this.Info.Skills.AddRange(this.skillManager.Infos);
        }
    }
}
