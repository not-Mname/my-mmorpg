using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipItem : MonoBehaviour, IPointerClickHandler
{
    public Text Name;
    public Text Level;
    public Sprite seletedIcon;
    public Text limitClass;
    public Text limitCategory;
    public Image BG;
    public Image Icon;

    Item item;
    UIEquip owner;
    bool isEquiped;
    int equipItemId;

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
            if(!isEquiped)
                BG.overrideSprite = selected ? seletedIcon : null;
        }
    }

    public void SetEquipItem(Item item, int EquipItemId, UIEquip Equip, bool Equiped)
    {
        this.owner = Equip;
        this.isEquiped = Equiped;
        this.equipItemId = EquipItemId;
        this.item = item;

        if(Name != null) Name.text = item.define.Name;
        if(Level!= null) Level.text = item.define.Level.ToString();
        if(limitClass!= null) limitClass.text = item.define.LimitClass.ToString();
        if(limitCategory!= null) limitCategory.text = item.define.Category.ToString();
        if(Icon != null) Icon.sprite = Resources.Load<Sprite>(item.define.Icon);
    }

    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isEquiped)
        {
            this.UnEquip();
        }
        else
        {
            if (this.Selected)
            {
                this.DoEquip();
                
                this.Selected = false;
            }
            else
            {
                this.owner.OnItemSelected(this);
                this.Selected = true;
            }
           
        }
    }

    private void DoEquip()
    {
        var mes = MessageBox.Show(string.Format("是否穿戴{0}？", item.define.Name), "确认", MessageBoxType.Confirm);
        mes.OnYes = () =>
        {
            var oldEquip = EquipManager.Instance.GetEquip(item.equipDefine.Slot);
            if (oldEquip!= null)
            {
                var newMes = MessageBox.Show(string.Format("是否替换{0}？", oldEquip.define.Name), "确认", MessageBoxType.Confirm);
                newMes.OnYes = () =>
                {
                    this.owner.DoEquip(item);
                };
            }
            else
            {
                this.owner.DoEquip(item);
            }
        };
    }

    private void UnEquip()
    {
        var mes = MessageBox.Show(string.Format("是否卸下{0}？", item.define.Name), "确认", MessageBoxType.Confirm);
        mes.OnYes = () =>
        {
            this.owner.UnEquip(item);
        };
    }
}
    

