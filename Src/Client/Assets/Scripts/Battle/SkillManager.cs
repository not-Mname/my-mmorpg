using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Battle
{
    public class SkillManager
    {
        private BattleUnit _owner;
        
        public List<Skill> Skills { get; private set; }

        public SkillManager(BattleUnit owner)
        {
            _owner = owner;
            Skills = new List<Skill>();
            this.InitSkills();
        }

        private void InitSkills()
        {
            this.Skills.Clear();
            foreach (var skill in _owner.Info.Skills)
            {
                Skill s = new Skill(skill, this._owner);
                AddSkill(s);
            }
            EVENT.Fire(Const.EventId.on_skill_updata);
        }

        public void AddSkill(Skill skill)
        {
            this.Skills.Add(skill);
        }

        public Skill GetSkill(int id)
        {
            return this.Skills.Find(s => s.Info.Id == id);
        }

        public void OnUpdate(float delta)
        {
            foreach (var skill in this.Skills)
            {
                skill.OnUpdate(delta);
            }
        }

        public void UpdateSkills()
        {
            foreach (var skillInfo in this._owner.Info.Skills)
            {
                Skill skill = this.GetSkill(skillInfo.Id);
                if (skill == null)
                {
                    this.AddSkill(skill);
                }
                else
                {
                    skill.Info = skillInfo;
                }
            }
            EVENT.Fire(Const.EventId.on_skill_updata);
        }
    }
}
