using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    public class ItemService : Singleton<ItemService>, IDisposable
    {
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<SkillBridge.Message.ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Unsubscribe<SkillBridge.Message.ItemEquipResponse>(this.OnItemEquip);
        }



        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<SkillBridge.Message.ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Subscribe<SkillBridge.Message.ItemEquipResponse>(this.OnItemEquip);
        }
        void OnItemEquip(object sender, ItemEquipResponse message)
        {
            if(message.Result == Result.Success)
            {
                if(pendingEquip != null)
                {
                    if (isEquip)
                    {
                        EquipManager.Instance.OnEquipItem(pendingEquip);
                    }
                    else
                    {
                        EquipManager.Instance.OnUnequipItem(pendingEquip.equipDefine.Slot);
                    }
                    pendingEquip = null;
                }
            }
        }

        void OnItemBuy(object sender, ItemBuyResponse message)
        {
            MessageBox.Show(string.Format("购买物品{0}！", message.Result));
        }

        public void SendBuyItem(int shopItemId, int shopId)
        {
            Debug.LogFormat("SendBuyItem {0} {1}", shopItemId, shopId);
            var message = new SkillBridge.Message.NetMessage();
            message.Request = new SkillBridge.Message.NetMessageRequest();
            message.Request.itemBuy = new SkillBridge.Message.ItemBuyRequest();

            message.Request.itemBuy.shopId = shopId;
            message.Request.itemBuy.shopItemId = shopItemId;

            NetClient.Instance.SendMessage(message);
        }

        Item pendingEquip = null;
        bool isEquip = false;
        public bool SendEquipItem(Item Equip, bool IsEquip)
        {
            if (pendingEquip != null) return false;//有未完成的请求

            pendingEquip = Equip;
            isEquip = IsEquip;

            var message = new SkillBridge.Message.NetMessage();
            message.Request = new SkillBridge.Message.NetMessageRequest();
            message.Request.itemEquip = new SkillBridge.Message.ItemEquipRequest();

            message.Request.itemEquip.isEquip = IsEquip;
            message.Request.itemEquip.itemId = Equip.id;
            message.Request.itemEquip.Slot = (int)Equip.equipDefine.Slot;

            NetClient.Instance.SendMessage(message);
            return true;
        }

        public void SendBagSave(NBagInfo data)
        {
            if (data == null && data.Items == null) return;
            var message = new SkillBridge.Message.NetMessage();
            message.Request = new SkillBridge.Message.NetMessageRequest();
            message.Request.bagSave = new SkillBridge.Message.BagSaveRequest();
            message.Request.bagSave.Bag = data;
            NetClient.Instance.SendMessage(message);
        }
    }
}
