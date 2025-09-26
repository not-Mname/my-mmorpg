using Common.Data;
using Const;
using Models;
using Services;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class UIShop : UIWindow
{
    public Transform[] itemRoot;
    public Text money;
    public Text title;
    public GameObject itemPrefab;
    ShopDefine shop;

    private UIShopItem shopItem;
    public void SelectShopItem(UIShopItem uiShopItem)
    {
        if (shopItem != null)
            shopItem.Selected = false;
        shopItem = uiShopItem;
    }

    IEnumerator Init()
    {
        int count = 0;
        int page = 0;
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            GameObject obj = Instantiate(itemPrefab, itemRoot[page]);
            UIShopItem uiShopItem = obj.GetComponent<UIShopItem>();
            uiShopItem.SetShopItem(kv.Value, this, kv.Key);
            count++;
            if(count >= 10)
            {
                count = 0;
                page++;
                itemRoot[page].gameObject.SetActive(true);
            }
        }
        yield return null;
        EVENT.Subscribe<int>(EventId.on_money_change, OnMoneyChange);
    }

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        title.text = shop.Name;
        money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
    }


///////////////////////////////////////回调////////////////////////////////////////////////////////

    void OnMoneyChange(int gold)
    {
        this.money.text = gold.ToString();
    }

    public override void OnCloseClick()
    {
        base.OnCloseClick();
        EVENT.Unsubscribe(EventId.on_money_change);
    }

    public void OnClickBuy()
    {
        if(shopItem == null)
        {
            MessageBox.Show("请选择商品");
            return;
        }
        ItemService.Instance.SendBuyItem(shopItem.shopItemId, shop.ID);
    }

    void Start()
    {
        StartCoroutine(Init());
        money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
    }

}
