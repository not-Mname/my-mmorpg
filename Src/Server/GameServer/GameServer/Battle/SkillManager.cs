using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System.Collections.Generic;


namespace Battle
{
    public class SkillManager
    {
        private BattleUnit _owner;
        
        public List<Skill> Skills { get; private set; }
        public List<NSkillInfo> Infos { get; private set; }

        public SkillManager(BattleUnit owner)
        {
            this._owner = owner;
            this.Skills = new List<Skill>();
            this.Infos = new List<NSkillInfo>();
            this.InitSkills();
        }

        private void InitSkills()
        {
            this.Skills.Clear();
            this.Infos.Clear();

            if (!DataManager.Instance.Skills.ContainsKey(this._owner.Define.TID))
            {
                return;
            }

            foreach (var define in DataManager.Instance.Skills[this._owner.Define.TID])
            {
                NSkillInfo info = new NSkillInfo();
                info.Id = define.Key;
                if(this._owner.Info.Level >= define.Value.UnlockLevel)
                {
                    info.Level = 5;
                }
                else
                {
                    info.Level = 1;
                }
                this.Infos.Add(info);
                Skill skill = new Skill(info, this._owner);
                this.AddSkill(skill);
            }
        }

        public void AddSkill(Skill skill)
        {
            this.Skills.Add(skill);
        }
    }
}
