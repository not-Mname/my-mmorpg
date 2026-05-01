using Common.Data;
using GameInterFace;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Scripting;

namespace Managers
{
    [Preserve]
    public class ShopManager : Singleton<ShopManager>, IInitializable, IDisposable
    {
        public void Init()
        {
            NPCManager.Instance.DegisterNPCEvent(Common.Data.NPCFunction.InvokeShop, OnOpenShop);
        }

        public bool OnOpenShop(NPCDefine npc)
        {
            this.ShowShop(npc.param);
            return true;
        
        }

        public void ShowShop(int param)
        {
            ShopDefine shop;
            if(DataManager.Instance.Shops.TryGetValue(param, out shop))
            {
                UIShop uiShop = UIManager.Instance.Show<UIShop>();
                if (uiShop != null)
                    uiShop.SetShop(shop);
            }
        }

        public bool BuyItem(int shopID, int shopItemId)
        {
            ItemService.Instance.SendBuyItem(shopItemId, shopID);
            return true;
        }

        public void Dispose()
        {
            NPCManager.Instance.DegisterNPCEvent(Common.Data.NPCFunction.InvokeShop, OnOpenShop);
        }
    }
}
