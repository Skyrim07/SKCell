using System.Collections.Generic;

namespace SKCell
{
    /// <summary>
    /// Heap with the max element on top.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaxHeap<T> 
    {
        private T[] heap;
        private int size = 0;
        private int capacity;
        private IComparer<T> comparer;

        public MaxHeap(int capacity, IComparer<T> comparer)
        {
            if (capacity < 1)
            {
                SKUtils.EditorLogError("MaxHeap: invalid capacity.");
            }

            if (heap == null)
            {
                heap = new T[capacity];
            }

            this.capacity = capacity;
            this.comparer = comparer;
        }

        public int GetSize()
        {
            return size;
        }

        public T[] GetHeap()
        {
            return heap;
        }

        public void Add(T node)
        {
            if (size == 0)
            {
                heap[0] = node;
                this.size++;
            }
            else if (this.size == this.capacity)
            {
                ProcessFullHeap(node);
            }
            else if (this.size < this.capacity)
            {
                heap[this.size] = node;

                int ParentPos = (this.size - 1) >> 1;
                int curPos = this.size;

                while (ParentPos > 0)
                {
                    if (comparer.Compare(heap[ParentPos], heap[curPos]) <= 0)
                    {
                        Swap(ref heap[ParentPos], ref heap[curPos]);
                        curPos = ParentPos;
                        ParentPos = (curPos - 1) >> 1;
                    }
                    else
                    {
                        break;
                    }
                }

                this.size++;
            }
        }

        public T GetTop()
        {
            if (this.size > 0)
            {
                return (T)heap[0];
            }
            SKUtils.EditorLogWarning("MaxHeap.GetTop: Heap is empty!");
            return default;
        }

        private void ProcessFullHeap(T node)
        {
            if (comparer.Compare(node, GetTop()) > 0)
            {
                return;
            }

            heap[0] = node;
            int curPos = 0;
            int left = (curPos << 1) + 1;
            int right = (curPos << 1) + 2;

            while (left < this.size)
            {
                T root = heap[curPos];

                if (right < this.size)
                {
                    if (comparer.Compare(heap[left], heap[right]) < 0)
                    {
                        if (comparer.Compare(heap[curPos], heap[right]) < 0)
                        {
                            Swap(ref heap[curPos], ref heap[left]);
                            curPos = right;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (comparer.Compare(heap[curPos], heap[left]) < 0)
                        {
                            Swap(ref heap[curPos], ref heap[left]);
                            curPos = left;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (comparer.Compare(heap[curPos], heap[left]) < 0)
                {
                    Swap(ref heap[curPos], ref heap[left]);
                }
                else
                {
                    break;
                }

                left = (curPos << 1) + 1;
                right = (curPos << 1) + 2;
            }

        }

        private void Swap(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }
}
