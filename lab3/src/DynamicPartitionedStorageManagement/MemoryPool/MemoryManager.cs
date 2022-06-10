namespace MemoryManager
{
    public abstract partial class MemoryManager : IMemoryManager
    {
        public nuint? AllocateMemory(int size)
        {
            var memory = AllocateMemoryImpl(size);
            if (memory.HasValue)
            {
                allocatedBlocks.Add(memory.Value, size);
            }
            return memory;
        }

        private bool IsAllocated(nuint memory)
        {
            return allocatedBlocks.ContainsKey(memory);
        }
        protected abstract nuint? AllocateMemoryImpl(int size);

        public bool FreeMemory(nuint memory)
        {
            if (!IsAllocated(memory))
            {
                return false;
            }
            FreeMemoryBlock(new MemoryBlock(memory, allocatedBlocks[memory]));
            allocatedBlocks.Remove(memory);
            return true;
        }

        protected (LinkedListNode<MemoryBlock>? predecessor, LinkedListNode<MemoryBlock>? successor)
            FindNearFreeBlocks(MemoryBlock memoryBlock)
        {
            LinkedListNode<MemoryBlock>? predecessor = null;
            LinkedListNode<MemoryBlock>? successor = null;

            nuint memory = memoryBlock.Memory;
            int size = memoryBlock.Size;
            bool isFirstBlock = IsFirstBlock(memoryBlock.Memory, memoryBlock.Size);
            bool isLastBlock = IsLastBlock(memoryBlock.Memory, memoryBlock.Size);

            for (var freeBlockNode = freeBlocks.First; freeBlockNode is not null; freeBlockNode = freeBlockNode.Next)
            {
                var freeBlock = freeBlockNode.Value;
                if (!isFirstBlock && freeBlock.Memory + (nuint)freeBlock.Size == memory)
                {
                    predecessor = freeBlockNode;
                    if (successor is not null)
                    {
                        break;
                    }
                }
                if (!isLastBlock && memory + (nuint)size == freeBlock.Memory)
                {
                    successor = freeBlockNode;
                    if (predecessor is not null)
                    {
                        break;
                    }
                }
            }

            return (predecessor, successor);
        }

        protected virtual void FreeMemoryBlock(MemoryBlock memoryBlock)
        {
            (var predecessor, var successor) = FindNearFreeBlocks(memoryBlock);
            if (predecessor is not null || successor is not null)
            {
                nuint startAddress = memoryBlock.Memory;
                int size = memoryBlock.Size;
                if (predecessor is not null)
                {
                    startAddress = predecessor.Value.Memory;
                    size += predecessor.Value.Size;
                    freeBlocks.Remove(predecessor);
                }
                if (successor is not null)
                {
                    size += successor.Value.Size;
                    freeBlocks.Remove(successor);
                }
                memoryBlock = new MemoryBlock(startAddress, size);
            }
            freeBlocks.AddLast(memoryBlock);
        }

        public LinkedList<MemoryBlockInfo> GetFreeMemories()
        {
            var memoryInfoList = new LinkedList<MemoryBlockInfo>();
            foreach (var memoryBlock in freeBlocks)
            {
                memoryInfoList.AddLast(new MemoryBlockInfo(memoryBlock.Memory, memoryBlock.Size));
            }
            return memoryInfoList;
        }

        private bool IsFirstBlock(nuint memory, int size)
        {
            return size > 0 && size <= memorySize && memory == startAddress;
        }

        private bool IsLastBlock(nuint memory, int size)
        {
            return size > 0 && size <= memorySize
                && memory >= startAddress && startAddress + (nuint)memorySize == memory + (nuint)size;
        }

        public MemoryManager(nuint memory, int size)
        {
            if (size <= 0)
            {
                throw new ArgumentException("The size of memory must be positive!");
            }
            if (memory > 0 && nuint.MaxValue - memory + 1 < (nuint)size)
            {
                throw new ArgumentException("Memory address out of bound!");
            }
            freeBlocks.AddLast(new MemoryBlock(memory, size));
            startAddress = memory;
            memorySize = size;
        }

        protected readonly LinkedList<MemoryBlock> freeBlocks = new LinkedList<MemoryBlock>();
        private readonly SortedList<nuint, int> allocatedBlocks = new SortedList<nuint, int>();
        private readonly nuint startAddress;
        private readonly int memorySize;
    }
}
