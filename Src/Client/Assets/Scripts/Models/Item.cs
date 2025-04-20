using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class Item
    {
        public int id { get; set; }
        public int count { get; set; }
        public ItemDefine define;
        public Item(NItemInfo info) : this(info.Id, info.Count) { }
        public EquipDefine equipDefine;

        public Item(int Id, int Count)
        {
            id = Id;
            count = Count;

            DataManager.Instance.Items.TryGetValue(this.id, out this.define);
            DataManager.Instance.Equips.TryGetValue(this.id, out this.equipDefine);
        }


        public override string ToString()
        {
            return string.Format("Item: {0} Count: {1}", id, count);
        }
    }
}
