using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Models
{
    public class Quest
    {
        public QuestDefine define;
        public NQuestInfo info;

        public Quest() { }

        public Quest(QuestDefine define)
        {
            this.define = define;
            this.info = null;
        }

        public Quest(NQuestInfo info)
        {
            this.info = info;
            this.define = DataManager.Instance.Quests[info.QuestId];
        }
    }
}
