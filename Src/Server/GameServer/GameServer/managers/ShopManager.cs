using Common;
using Common.Data;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class ShopManager : Singleton<ShopManager>
    {
        public Result BuyItem(NetConnection<NetSession> sender, int shopId, int shopItemId)
        {
            if (!DataManager.Instance.ShopItems[shopId].ContainsKey(shopItemId))
            {
                return Result.Failed;
            }
            ShopItemDefine item;
            if (DataManager.Instance.ShopItems[shopId].TryGetValue(shopItemId, out item))
            {
                Log.InfoFormat("BuyItem : character {0} buy item {1} count{2} price{3}", 
                    sender.Session.Character.Data.Name, item.ItemID, item.Count, item.Price);

                if(sender.Session.Character.Data.Gold >= item.Price)
                {
                    sender.Session.Character.itemManager.AddItem(item.ItemID, item.Count);
                    sender.Session.Character.Gold -= item.Price;
                    DBService.Instance.Save();
                    return Result.Success;
                }               
            }
            return Result.Failed;
        }
    }
}