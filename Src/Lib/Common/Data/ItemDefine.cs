using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public enum ItemFunction
    {
        RecoverHp,
        RecoverMp,
        AddBuff,
        AddExp,
        AddMoney,
        AddItem,
        AddSkillPoint,
    }

    public class ItemDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public string Category { get; set; }
        public bool CanUse { get; set; }
        public int Price { get; set; }
        public int SellPrice { get; set; }
        public ItemFunction Function { get; set; }
        public int Param1 { get; set; }
        public int StackLimit { get; set; }
        public string Icon { get; set; }

        public int Level { get; set; }
        public CharacterClass LimitClass { get; set; }

    }
}
