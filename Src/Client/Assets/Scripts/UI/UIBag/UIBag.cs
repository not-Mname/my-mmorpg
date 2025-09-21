using Managers;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow
{
    public UITabView tabView;
    public Text money;
    public GameObject bagItem;
    List<Image> slots;
    int oneTabSlotCount = 35;

    public override void OnCloseClick()
    {
        base.OnCloseClick();
        ItemService.Instance.SendBagSave(BagManager.Instance.GetBagInfo());
    }

    public void OnClickReset()
    {
        BagManager.Instance.Reset();       
    }

    void Start()
    {
        for (int i = 0; i < tabView.pageControler.Length; i++)
        {
            if (slots == null)
            {
                slots = new List<Image>();
            }
            slots.AddRange(tabView.pageControler[i].content.GetComponentsInChildren<Image>(true));
            
            StartCoroutine(InitBags());
        }
        money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
    }

    IEnumerator InitBags()
    {

        for (int i = 0; i < BagManager.Instance.items.Length; i++)
        {
            if (BagManager.Instance.items[i].itemId == 0) continue;
            var item = Instantiate(bagItem, slots[i].transform);
            UIIconItem iconItem = item.GetComponent<UIIconItem>();
            var define = DataManager.Instance.Items[BagManager.Instance.items[i].itemId];
            var test = BagManager.Instance.items[i].count.ToString();
            iconItem.SetMainIcon(define.Icon, test);
        }
        for (int i = BagManager.Instance.items.Length; i < slots.Count; i++)
        {
            slots[i].color = Color.gray;
        }
        yield return null;
    }
}
