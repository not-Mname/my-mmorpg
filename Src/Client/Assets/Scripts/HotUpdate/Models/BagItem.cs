using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Assets.Scripts.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]//设置结构体的布局为顺序排列，并且按照1字节对齐
    public struct BagItem
    {
        public ushort itemId;
        public ushort count;

        public static BagItem zero = new BagItem(0, 0);

        BagItem(ushort Id, ushort Count)
        {
            this.count = Count;
            this.itemId = Id;
        }

        public static bool operator ==(BagItem lhs, BagItem rhs)
        {
            return lhs.itemId == rhs.itemId && lhs.count == rhs.count;
        }

        public static bool operator !=(BagItem lhs, BagItem rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj is BagItem) return Equals((BagItem)obj);
            return false;
        }

        public bool Equals(BagItem other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return itemId.GetHashCode() ^ count.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("itemId:{0}, count:{1}", itemId, count);
        }
    }

}
