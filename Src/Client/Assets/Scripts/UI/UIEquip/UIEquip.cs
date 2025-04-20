using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquip : UIWindow
{
    public Transform itemListRoot;
    public Text title;
    public Text money;
    public GameObject itemPrfab;
    public GameObject itemEquipPrfab;
    public UIEquipItem selectedItem;

    public List<Transform> slots;

    void Start()
    {
        RefreshUI();
        EquipManager.Instance.OnEquipChanged += RefreshUI;
    }

    void OnDestroy()
    {
        EquipManager.Instance.OnEquipChanged -= RefreshUI;
    }

    public void OnItemSelected(UIEquipItem item)
    { 
        if (selectedItem != null)
        {
            selectedItem.Selected = false;
            selectedItem = item;
        }
    }

    void RefreshUI()
    {
        this.ClearAllEquipList();
        this.InitAllEquipList();
        this.ClearAllEquipedList();
        this.InitAllEquipedList();
        money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    public void DoEquip(Item item)
    {
        EquipManager.Instance.EquipItem(item);
    }
    public void UnEquip(Item item)
    {
        EquipManager.Instance.UnEquipItem(item);
    }

    private void InitAllEquipedList()
    {
        for (int i = 0; i < (int)SkillBridge.Message.EquipSlot.SlotMax; i++)
        {
            var item = EquipManager.Instance.equips[i];
            if (item == null) continue;
            GameObject itemObj = Instantiate(itemPrfab, slots[i]);
            UIEquipItem itemUI = itemObj.GetComponent<UIEquipItem>();
            itemUI.SetEquipItem(item, item.id, this, true);
        }
    }

    private void ClearAllEquipedList()
    {
        foreach (var slot in slots)
        {
            if (slot.childCount > 1)
                Destroy(slot.GetChild(1).gameObject);
        }
    }

    private void InitAllEquipList()
    {
        foreach (var kv in ItemManager.Instance.itemInfos)
        {
            if (kv.Value.define.Type == SkillBridge.Message.ItemType.Equip)
            {
                if (EquipManager.Instance.Contains(kv.Key)) continue;
                GameObject itemObj = Instantiate(itemEquipPrfab, itemListRoot);
                UIEquipItem item = itemObj.GetComponent<UIEquipItem>();
                item.SetEquipItem(kv.Value, kv.Key, this, false);
            }
        }
    }

    private void ClearAllEquipList()
    {
        foreach (var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
        {
            Destroy(item.gameObject);
        }
    }
}
