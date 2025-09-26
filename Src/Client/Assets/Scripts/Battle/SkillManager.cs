using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            
        }
    }
}
