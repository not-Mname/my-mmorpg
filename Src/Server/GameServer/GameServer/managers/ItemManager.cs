using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class ItemManager
    {
        Character owner;
        public Dictionary<int, Item> items = new Dictionary<int, Item>();

        public ItemManager(Character Char)
        {
            this.owner = Char;

            foreach (var item in owner.Data.Items)
            {
                items.Add(item.ItemId, new Item(item));
            }
        }

        public bool UseItem(int itemId, int count = 1)
        {
            Log.InfoFormat("[{0}] used item {1} : {2}", owner.Data.ID, itemId, count);
            if (items.TryGetValue(itemId, out Item item))
            {
                if (item.count < count) return false;
                item.Remove(count);
                return true;
            }

            return false;
        }

        public bool HasItem(int itemId)
        {
            if (items.TryGetValue(itemId, out Item item))
            {
                return item.count > 0;
            }
            return false;
        }

        public Item GetItem(int itemId)
        {
            Log.InfoFormat("[{0}] get item {1}", owner.Data.ID, itemId);
            Item item = null;
            items.TryGetValue(itemId, out item);
            return item;

        }

        public bool AddItem(int itemId, int count = 1)
        {
            Item item = null;

            if (items.ContainsKey(itemId))
            {
                items[itemId].Add(count);

            }
            else
            {
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.ItemId = itemId;
                dbItem.ItemCount = count;
                dbItem.TCharacterID = this.owner.Info.EntityId;
                dbItem.Owner = owner.Data;
                owner.Data.Items.Add(dbItem);
                item = new Item(dbItem);
                items[itemId] = item;
            }
            Log.InfoFormat("[{0}] add item {1} : {2}", owner.Data.ID, itemId, count);
            this.owner.statusManager.AddItemChange(itemId, count, StatusAction.Add);
            //DBService.Instance.Save();
            return true;
        }

        public bool Remove(int itemId, int count = 1)
        {
            if (!items.ContainsKey(itemId)) return false;
            Item item = items[itemId];
            if (item.count < count) return false;
            item.Remove(count);
            this.owner.statusManager.AddItemChange(itemId, count, StatusAction.Delete);
            Log.InfoFormat("[{0}] remove item {1} : {2}", owner.Data.ID, itemId, count);
            //DBService.Instance.Save();
            return true;
        }

        public void GetItemInfos(List<NItemInfo> list)
        {
            foreach (var item in items)
            {
                list.Add(new NItemInfo() { Id = item.Key, Count = item.Value.count });
            }
        }
    }
}
