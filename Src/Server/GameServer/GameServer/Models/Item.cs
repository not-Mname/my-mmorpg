using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Item
    {
        public TCharacterItem dbItem;
        public int itemID;
        public int count;

        public Item(TCharacterItem item)
        {
            this.dbItem = item;
            this.itemID = (short)item.ItemId;
            this.count = (short)item.ItemCount;
        }

        public void Add(int count)
        {
            this.count += count;
            this.dbItem.ItemCount = this.count;
        }

        public void Remove(int count)
        {
            if (this.count < count) return;
            this.count -= count;
            this.dbItem.ItemCount = this.count;
        }

        public bool Use()
        {
            return false;
        }

        public override string ToString()
        {
            return string.Format("ItemID: {0}, Count: {1}", this.itemID, this.count);
        }
    }
}
