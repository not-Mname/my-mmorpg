using Assets.Scripts.Models;
using Google.Protobuf;
using SkillBridge.Message;
using System.Linq;

namespace Managers
{
    public class BagManager : Singleton<BagManager>
    {
        public int unlocked;

        public BagItem[] items;

        NBagInfo bagInfo;
        unsafe public void Init(NBagInfo bag)
        {
            if (bag == null) return;
            this.bagInfo = bag;
            this.unlocked = bag.Unlocked;
            items = new BagItem[this.unlocked];
            
            if(this.bagInfo.Items!= null && this.bagInfo.Items.Length >= this.unlocked)
            {
                Analyze(this.bagInfo.Items.ToArray());
            }
            else
            {
                this.bagInfo.Items = ByteString.CopyFrom(new byte[sizeof(BagItem) * this.unlocked]);
                Reset();
            }

        }

        public void Reset()
        {
            int index = 0;
            foreach ( var item in ItemManager.Instance.itemInfos)
            {
                if(item.Value.count <= item.Value.define.StackLimit)
                {
                    items[index].itemId = (ushort)item.Key;
                    items[index].count = (ushort)item.Value.count;
                }
                else
                {
                    int count = item.Value.count;
                    
                    while(count > item.Value.define.StackLimit)
                    {
                        items[index].itemId = (ushort)item.Key;
                        items[index].count = (ushort)item.Value.define.StackLimit;
                        count -= item.Value.define.StackLimit;
                        index++;
                    }
                    items[index].itemId = (ushort)item.Key;
                    items[index].count = (ushort)count;
                }
                index++;
            }
            
        }

        public void AddItem(int id, int value)
        {
            ushort addCount = (ushort)value;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].itemId == (ushort)id)
                {
                    ushort canAdd = (ushort)(DataManager.Instance.Items[items[i].itemId].StackLimit - items[i].count);
                    if (canAdd >= addCount)
                    {
                        items[i].count += addCount;
                        addCount = 0;
                        break;
                    }
                    else
                    {
                        items[i].count += canAdd;
                        addCount -= canAdd;
                    }  
                }
            }
            if (addCount > 0)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].itemId == 0)//寻找空位
                    {
                        items[i].itemId = (ushort)id;
                        items[i].count = addCount;
                        break;
                    }
                }
            }
        }

        public void RemoveItem(int id, int value)
        {
            
        }

        unsafe void Analyze(byte[] data)//解析背包数据
        {
            fixed (byte* ptr = data)
            {
                for(int i = 0; i < this.unlocked; i++)
                {
                    BagItem* item = (BagItem*)(ptr + i * sizeof(BagItem));
                    items[i] = *item;
                }
            }
        }

        unsafe public NBagInfo GetBagInfo()
        {
            fixed (byte* ptr = bagInfo.Items.ToArray())
            {
                for(int i = 0; i < this.unlocked; i++)
                {
                    BagItem* item = (BagItem*)(ptr + i * sizeof(BagItem));
                    *item = items[i];
                }
            }
            return this.bagInfo;
        }
    }
}
