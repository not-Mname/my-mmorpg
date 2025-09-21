using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;

namespace GameServer.Services
{
    public class ItemService : Singleton<ItemService>
    {
        public void Init()
        {
            Log.Info("ItemService Init...");
        }

        public ItemService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemBuyRequest>(this.OnItemBuy);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemEquipRequest>(this.OnItemEquip);
        }

        void OnItemEquip(NetConnection<NetSession> sender, ItemEquipRequest message)
        {
            Character cha = sender.Session.Character;
            Log.InfoFormat("ItemEquipRequest: Character {0}: Slot {1} Item {2} Equip{3}", cha.Id, message.Slot, message.itemId, message.isEquip);
            var result = EquipManager.Instance.EquipItem(sender, message.Slot, message.itemId, message.isEquip);
            sender.Session.Response.itemEquip = new ItemEquipResponse();
            sender.Session.Response.itemEquip.Result = result;
            sender.SendResponse();
        }

        void OnItemBuy(NetConnection<NetSession> sender, ItemBuyRequest message)
        {
            Character cha = sender.Session.Character;
            Log.InfoFormat("ItemBuyRequest: Character {0} Shop {1} shopItem {2}", cha.Id, message.shopId, message.shopItemId);
            var result = ShopManager.Instance.BuyItem(sender, message.shopId, message.shopItemId);
            sender.Session.Response.itemBuy = new ItemBuyResponse();
            sender.Session.Response.itemBuy.Result = result;
            sender.SendResponse();
        }
    }
}
