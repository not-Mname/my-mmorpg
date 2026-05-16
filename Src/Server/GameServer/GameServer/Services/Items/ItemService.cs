using Common;
using GameInterFace;
using GameServer.Entities;
using GameServer.Managers.Items;
using Network;
using SkillBridge.Message;
using System;

namespace GameServer.Services.Items
{
    public class ItemService : Singleton<ItemService>, IDisposable, IInitializable
    {
        public void Init()
        {
            Log.Info("ItemService Init...");
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemBuyRequest>(this.OnItemBuy);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemEquipRequest>(this.OnItemEquip);
        }

        public void Dispose()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<ItemBuyRequest>(this.OnItemBuy);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<ItemEquipRequest>(this.OnItemEquip);
        }

        void OnItemEquip(NetConnection<NetSession> sender, ItemEquipRequest message)
        {
            Character cha = sender.Session.Character;
            Log.InfoFormat("ItemEquipRequest: Character {0}: Slot {1} Item {2} Equip{3}", cha.Id, message.Slot, message.ItemId, message.IsEquip);
            var result = EquipManager.Instance.EquipItem(sender, message.Slot, message.ItemId, message.IsEquip);
            sender.Session.Response.ItemEquip = new ItemEquipResponse();
            sender.Session.Response.ItemEquip.Result = result;
            sender.SendResponse();
        }

        void OnItemBuy(NetConnection<NetSession> sender, ItemBuyRequest message)
        {
            Character cha = sender.Session.Character;
            Log.InfoFormat("ItemBuyRequest: Character {0} Shop {1} ShopItem {2}", cha.Id, message.ShopId, message.ShopItemId);
            var result = ShopManager.Instance.BuyItem(sender, message.ShopId, message.ShopItemId);
            sender.Session.Response.ItemBuy = new ItemBuyResponse();
            sender.Session.Response.ItemBuy.Result = result;
            sender.SendResponse();
        }
    }
}
