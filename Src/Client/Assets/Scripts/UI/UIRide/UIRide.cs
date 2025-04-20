using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


class UIRide : UIWindow
{
    public Text Descroption;
    public GameObject ItemPrefab;
    public ListView ListMain;
    private UIRideItem SelectedItem;

    void Start()
    {
        RefreshUI();
        this.ListMain.onItemSelected += OnItemSelected;
    }

    private void RefreshUI()
    {
        ClearItems();
        InitItems();
    }

    private void InitItems()
    {
        foreach (var item in ItemManager.Instance.itemInfos)
        {
            if(item.Value.define.Type == ItemType.Ride &&
                item.Value.define.LimitClass == CharacterClass.None ||
                item.Value.define.LimitClass == User.Instance.CurrentCharacter.Class)
            {
                if (EquipManager.Instance.Contains(item.Key))
                    continue;
                GameObject go = Instantiate(this.ItemPrefab, this.ListMain.transform);
                UIRideItem uiItem = go.GetComponent<UIRideItem>();
                uiItem.SetRideItem(item.Value);
                this.ListMain.AddItem(uiItem);
            }
        }
    }

    private void ClearItems()
    {
        ListMain.RemoveAll();
    }

    private void OnItemSelected(ListView.ListViewItem item)
    {
        this.SelectedItem = item as UIRideItem;
        this.Descroption.text = SelectedItem.Item.define.Description;
    }

    public void DoRide()
    {
        if(SelectedItem == null)
        {
            MessageBox.Show("请先选择一个骑乘道具");
            return; 
        }
        User.Instance.Ride(this.SelectedItem.Item.id);
    }
    public void CancelRide()
    {
        if(User.Instance.CurrentRide == 0)
        {
            MessageBox.Show("你还没有骑乘任何道具");
            return;
        
        }
        User.Instance.Ride(0);
    }
}

