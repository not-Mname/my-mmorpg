using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;

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
