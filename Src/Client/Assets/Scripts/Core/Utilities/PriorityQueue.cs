using System;
using System.Collections.Generic;

namespace Utilities
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> data;

        public int Count { get { return data.Count; } }

        public PriorityQueue()
        {
            this.data = new List<T>();
        }

        public void Enqueue(T item)
        {
            data.Add(item);                    // 1. 新元素加到末尾
            int childIndex = data.Count - 1;   // 当前新元素的索引

            while (childIndex > 0)
            {
                int parentIndex = (childIndex - 1) / 2;  // 父节点索引

                // 如果当前节点 >= 父节点，满足最小堆性质，停止上浮
                if (data[childIndex].CompareTo(data[parentIndex]) >= 0)
                    break;

                // 否则交换父子位置（上浮）
                T tmp = data[childIndex];
                data[childIndex] = data[parentIndex];
                data[parentIndex] = tmp;

                // 更新索引，继续向上检查
                childIndex = parentIndex;
            }
        }

        public T Dequeue()
        {
            int lastIndex = data.Count - 1;
            T frontItem = data[0];        // 保存堆顶元素（最高优先级）
            data[0] = data[lastIndex];    // 把最后一个元素移到根节点
            data.RemoveAt(lastIndex);     // 删除最后一个元素

            lastIndex--;                  // 堆大小减一
            int parentIndex = 0;          // 从根开始下沉

            while (true)
            {
                int childIndex = parentIndex * 2 + 1;  // 左子节点
                if (childIndex > lastIndex) break;     // 没有左孩子，结束

                int rightChild = childIndex + 1;
                // 如果右孩子存在且比左孩子更小，则选择右孩子
                if (rightChild <= lastIndex && data[rightChild].CompareTo(data[childIndex]) < 0)
                    childIndex = rightChild;

                // 如果父节点已经 <= 子节点，满足最小堆，停止
                if (data[parentIndex].CompareTo(data[childIndex]) <= 0)
                    break;

                // 否则交换父子（下沉）
                T tmp = data[parentIndex];
                data[parentIndex] = data[childIndex];
                data[childIndex] = tmp;

                // 继续向下调整
                parentIndex = childIndex;
            }
            return frontItem;
        }

        public T Peek()
        {
            return data[0];
        }

        public void Clear()
        {
            data.Clear();
        }
    }
}
