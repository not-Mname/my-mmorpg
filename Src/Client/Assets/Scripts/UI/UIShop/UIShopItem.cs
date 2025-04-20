using Common.Data;
using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour, ISelectHandler 
{
    public Text Name;
    public Text count;
    public Text price;
    public Image icon;
    public Image BG;
    public Sprite seletedIcon;
    public ShopItemDefine shopItem;
    public Text limitClass;

    UIShop shop;
    public int shopItemId;
    public ItemDefine item;
    

    private bool selected = false;
    public bool Selected
    {
        get
        {
            return this.selected;
        }
        set 
        {
            this.selected = value;
            BG.overrideSprite = selected? seletedIcon : null;
        } 
    }

    public void SetShopItem(ShopItemDefine Define, UIShop Owner, int Id)
    {
        this.shop = Owner;
        this.shopItemId = Id;
        this.shopItem = Define;

        this.item = DataManager.Instance.Items[Define.ItemID];
        this.Name.text = item.Name;
        this.icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);
        this.count.text = "x" + Define.Count.ToString();
        //this.limitClass.text = item.LimitClass.ToString();
        this.price.text = Define.Price.ToString();
        
    }

    public void OnSelect(BaseEventData eventData)
    {
        this.Selected = true;
        shop.SelectShopItem(this);
    }
}
