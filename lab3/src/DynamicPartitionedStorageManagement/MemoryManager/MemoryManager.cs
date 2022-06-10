using Utils;

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

            var afterBlockNode = freeBlocks.LowerBound(memoryBlock);
            LinkedListNode<MemoryBlock>? beforeBlockNode = null;
            if (afterBlockNode is not null)
            {
                if (!isLastBlock && memory + (nuint)size == afterBlockNode.Value.Memory)
                {
                    successor = afterBlockNode;
                }
                beforeBlockNode = afterBlockNode.Previous;
            }
            else
            {
                beforeBlockNode = freeBlocks.Last;
            }
            if (beforeBlockNode is not null)
            {
                if (!isFirstBlock && beforeBlockNode.Value.Memory + (nuint)beforeBlockNode.Value.Size == memory)
                {
                    predecessor = beforeBlockNode;
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
            freeBlocks.Add(memoryBlock);
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

        public LinkedList<MemoryBlockInfo> GetAllocatedMemories()
        {
            var memoryInfoList = new LinkedList<MemoryBlockInfo>();
            foreach (var memoryBlock in allocatedBlocks)
            {
                memoryInfoList.AddLast(new MemoryBlockInfo(memoryBlock.Key, memoryBlock.Value));
            }
            return memoryInfoList;
        }

        private bool IsFirstBlock(nuint memory, int size)
        {
            return size > 0 && size <= MemorySize && memory == StartAddress;
        }

        private bool IsLastBlock(nuint memory, int size)
        {
            return size > 0 && size <= MemorySize
                && memory >= StartAddress && StartAddress + (nuint)MemorySize == memory + (nuint)size;
        }

        public MemoryManager(nuint memory, int size)
        {
            if (size < 0)
            {
                throw new ArgumentException("The size of memory must be non-negative!");
            }
            if (memory > 0 && nuint.MaxValue - memory + 1 < (nuint)size)
            {
                throw new ArgumentException("Memory address out of bound!");
            }
            if (size != 0)
            {
                freeBlocks.Add(new MemoryBlock(memory, size));
            }
            StartAddress = memory;
            MemorySize = size;
        }

        protected readonly SortedLinkedList<MemoryBlock> freeBlocks = new SortedLinkedList<MemoryBlock>(new MemoryBlockComparer());
        private readonly SortedDictionary<nuint, int> allocatedBlocks = new SortedDictionary<nuint, int>();
        public nuint StartAddress { get; init; }
        public int MemorySize { get; init; }
    }
}
