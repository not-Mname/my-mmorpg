using Common.Battle;
using Const;
using Managers;
using Models;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class UIEquip : UIWindow
{
    public Transform itemListRoot;
    public Text title;
    public Text money;
    public TextMeshProUGUI HP;
    public TextMeshProUGUI MP;
    public GameObject itemPrfab;
    public GameObject itemEquipPrfab;
    public UIEquipItem selectedItem;
    public Scrollbar HPBar;
    public Scrollbar MPBar;

    public List<Transform> slots;
    public List<TextMeshProUGUI> Attributes;

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
        this.InitAttributes();
        money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
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
            {
                Destroy(slot.GetChild(1).gameObject);
            }
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

    private void InitAttributes()
    {
        var chaAtt = User.Instance.CurrentCharacter;
        var cha = User.Instance.CurrentCharacterInfo.Dynamic;

        this.HP.text = string.Format("{0}/{1}", cha.Hp, chaAtt.Attributes.Final.Data[(int)AttributesType.MaxHp].ToString());
        this.HP.text = string.Format("{0}/{1}", cha.Mp, chaAtt.Attributes.Final.Data[(int)AttributesType.MaxMp].ToString());
        this.HPBar.size = cha.Hp / chaAtt.Attributes.Final.Data[(int)AttributesType.MaxHp];
        this.MPBar.size = cha.Mp / chaAtt.Attributes.Final.Data[(int)AttributesType.MaxMp];

        for (int i = (int)AttributesType.STR; i < (int)AttributesType.Max; i++)
        {
            if (i != (int)AttributesType.CRI)
            {
                Attributes[i - (int)AttributesType.STR].text = chaAtt.Attributes.Final.Data[i].ToString();
            }
            else
            {
                Attributes[i - (int)AttributesType.STR].text = (chaAtt.Attributes.Final.Data[i] * 100).ToString() + "%";
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
