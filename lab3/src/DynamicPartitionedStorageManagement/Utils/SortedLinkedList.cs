namespace Utils
{
    public class SortedLinkedList<T>
        : ICollection<T>, IEnumerable<T>, System.Collections.IEnumerable, IReadOnlyCollection<T>, System.Collections.ICollection
    {
        public SortedLinkedList(IComparer<T> comparer)
        {
            list = new LinkedList<T>();
            this.comparer = comparer;
        }

        public SortedLinkedList(IEnumerable<T> collection, IComparer<T> comparer)
        {
            list = new LinkedList<T>(collection);
            this.comparer = comparer;
        }

        public void Clear()
        {
            list.Clear();
        }

        public void Add(T item)
        {
            AddAndGetNode(item);
        }

        public LinkedListNode<T> AddAndGetNode(T item)
        {
            var firstItem = list.First;
            if (firstItem == null)
            {
                return list.AddLast(item);
            }
            else if (comparer.Compare(item, firstItem.Value) < 0)
            {
                return list.AddFirst(item);
            }
            else
            {
                for (; firstItem.Next != null; firstItem = firstItem.Next)
                {
                    if (comparer.Compare(item, firstItem.Next.Value) < 0)
                    {
                        return list.AddAfter(firstItem, item);
                    }
                }
                return list.AddLast(item);
            }
        }

        public bool Contains(T item)
        {
            foreach (var listItem in list)
            {
                int res = comparer.Compare(item, listItem);
                if (res == 0)
                {
                    return true;
                }
                if (res < 0)
                {
                    break;
                }
            }
            return false;
        }

        public LinkedListNode<T>? LowerBound(T item)
        {
            for (var itemNode = list.First; itemNode != null; itemNode = itemNode.Next)
            {
                int res = comparer.Compare(item, itemNode.Value);
                if (res <= 0)
                {
                    return itemNode;
                }
            }
            return null;
        }

        public LinkedListNode<T>? UpperBound(T item)
        {
            for (var itemNode = list.First; itemNode != null; itemNode = itemNode.Next)
            {
                int res = comparer.Compare(item, itemNode.Value);
                if (res < 0)
                {
                    return itemNode;
                }
            }
            return null;
        }

        public bool Remove(T item) => list.Remove(item);
        public void Remove(LinkedListNode<T> item) => list.Remove(item);

        public LinkedListNode<T>? First => list.First;
        public LinkedListNode<T>? Last => list.Last;

        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        void System.Collections.ICollection.CopyTo(Array array, int index) => list.CopyTo((T[])array, index);

        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => list.GetEnumerator();

        public bool IsReadOnly => false;
        public bool IsSynchronized => false;
        public object SyncRoot { get; } = new object();
        public int Count => list.Count();
        private LinkedList<T> list;
        private IComparer<T> comparer;
    }
}
