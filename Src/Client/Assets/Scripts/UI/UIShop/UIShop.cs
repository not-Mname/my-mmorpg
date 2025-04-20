using Common.Data;
using Models;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        title.text = shop.Name;
        money.text = User.Instance.CurrentCharacter.Gold.ToString();
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
        money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }


    void Update()
    {

    }
}
