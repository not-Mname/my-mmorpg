using Common.Data;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChanegeHandler();

        public OnEquipChanegeHandler OnEquipChanged;

        public Item[] equips = new Item[(int)EquipSlot.SlotMax];

        byte[] data;

        public void Init(byte[] data)
        {
            this.data = data;
            this.ParseEquipData(data);
        }

        unsafe void ParseEquipData(byte[] data)
        {
            fixed (byte* ptr = data)
            {
                for(int i = 0; i < equips.Length; i++)
                {
                    int id = *(int*)(ptr + i * sizeof(int));
                    if(id > 0)
                        this.equips[i] = ItemManager.Instance.itemInfos[id];
                    else
                        this.equips[i] = null;
                }
            }
        }

        public Item GetEquip(EquipSlot slot)
        {
            return this.equips[(int)slot];
        }

        unsafe public byte[] GetEquipData()
        {
            fixed (byte* ptr = data)
            {
                for (int i = 0; i < equips.Length; i++)
                {
                    int* id = (int*)(ptr + i * sizeof(int));
                    if (this.equips[i] != null)
                    {
                        *id = this.equips[i].id;
                    }
                    else
                    {
                        *id = 0;
                    }
                }
            }
            return data;
        }

        unsafe public bool Contains(int equipId)
        {
            for(int i = 0; i < equips.Length; i++)
            {
                if(equips[i] != null && equips[i].id == equipId)
                    return true;
            }
            return false;
        }

        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, true);
        }
        public void UnEquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, false);
        }

        public void OnEquipItem(Item equip)
        {
            if (this.equips[(int)equip.equipDefine.Slot] != null && this.equips[(int)equip.equipDefine.Slot].id == equip.id) return;
            this.equips[(int)equip.equipDefine.Slot] = equip;
            if (OnEquipChanged != null)
                OnEquipChanged();   

        }

        public void OnUnequipItem(EquipSlot equipSlot)
        {
            if (this.equips[(int)equipSlot] != null)
            {
                this.equips[(int)equipSlot] = null;
                if (OnEquipChanged != null)
                    OnEquipChanged();
            }
        }

        public List<EquipDefine> GetEquipDefines()
        {
            var result = new List<EquipDefine>();
            foreach (var item in equips)
            {
               if(item != null)
                {
                    result.Add(item?.equipDefine);
                }
            }
            return result;
        }
    }
}
