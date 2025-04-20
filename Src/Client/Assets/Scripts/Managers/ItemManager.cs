using Common.Data;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    public class ItemManager : Singleton<ItemManager>
    {
        public Dictionary<int, Item> itemInfos = new Dictionary<int, Item>();
        public void Init(List<NItemInfo> items)
        {
            itemInfos.Clear();
            foreach (var item in items)
            {
                itemInfos[item.Id] = new Item(item);
            }

            StatusService.Instance.RegiterStautsNotify(StatusType.Item, OnItemNotify);
        }

        public bool OnItemNotify(NStatus status)
        {
            if (status.Action == StatusAction.Add)
            {
                this.AddItem(status.Id, status.Value);
            }
            else if (status.Action == StatusAction.Delete)
            {
                this.RemoveItem(status.Id, status.Value);
            }

            return false;
        }

        private void RemoveItem(int id, int value)
        {
            Item item = null;
            if (!itemInfos.TryGetValue(id, out item) || item.count < value)
            {
                return;
            }

            item.count -= value;

            BagManager.Instance.RemoveItem(id, value);

        }

        private void AddItem(int id, int value)
        {
            Item item = null;
            if (itemInfos.TryGetValue(id, out item))
            {
                item.count += value;
            }
            else
            {
                this.itemInfos[id] = new Item(id, value);
                this.itemInfos[id].count = value;
            }
            BagManager.Instance.AddItem(id, value);
        }

        public ItemDefine GetItemDefine(int itemId)
        {
            return default(ItemDefine);
        }
        public bool UseItem(int itemId)
        {
            return false;
        }
        public bool UseItem(ItemDefine item)
        {
            return false;
        }

    }
}
