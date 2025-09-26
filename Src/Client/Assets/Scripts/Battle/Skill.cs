using Common.Data;
using Entities;

namespace Battle
{
    public class Skill
    {
        public NSkillInfo Info { get; set; }
        public BattleUnit Owner { get; set; }
        public SkillDefine Define { get; set; }

        public Skill(NSkillInfo info, BattleUnit owner)
        {
            this.Info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][info.Id];
        }
    }
}
