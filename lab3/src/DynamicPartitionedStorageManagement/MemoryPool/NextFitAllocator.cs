namespace MemoryManager
{
    internal class NextFitAllocator : MemoryManager
    {
        protected override nuint? AllocateMemoryImpl(int size)
        {
            if (size <= 0)
            {
                return null;
            }

            if (lastAllocatePosition is null)
            {
                return null;
            }

            LinkedListNode<MemoryBlock> itr = lastAllocatePosition;
            do
            {
                if (itr.Value.Size >= size)
                {
                    nuint ans = itr.Value.Memory;
                    if (itr.Value.Size == size)
                    {
                        lastAllocatePosition = freeBlocks.Count == 1 ? null :
                            itr.Next ?? freeBlocks.First;
                        freeBlocks.Remove(itr);
                    }
                    else
                    {
                        itr.ValueRef = new MemoryBlock(itr.Value.Memory + (nuint)size, itr.Value.Size - size);
                    }
                    return ans;
                }

                itr = itr.Next ?? freeBlocks.First
                    ?? throw new Exception("This code shouldn't be reachable.");    // lastAllocatePosition Means freeBlocks isn't empty

            } while (ReferenceEquals(itr, lastAllocatePosition));

            return null;
        }

        protected override void FreeMemoryBlock(MemoryBlock memoryBlock)
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

                    if (ReferenceEquals(predecessor, lastAllocatePosition))
                    {
                        lastAllocatePosition = freeBlocks.Count == 1 ? null :
                            lastAllocatePosition.Next ?? freeBlocks.First;
                    }

                    freeBlocks.Remove(predecessor);
                }

                if (successor is not null)
                {
                    size += successor.Value.Size;

                    if (ReferenceEquals(successor, lastAllocatePosition))
                    {
                        lastAllocatePosition = freeBlocks.Count == 1 ? null :
                            lastAllocatePosition.Next ?? freeBlocks.First;
                    }

                    freeBlocks.Remove(successor);
                }

                freeBlocks.AddLast(new MemoryBlock(startAddress, size));
                lastAllocatePosition = lastAllocatePosition ?? freeBlocks.First;
            }
            else
            {
                freeBlocks.AddLast(memoryBlock);
            }
        }

        public NextFitAllocator(nuint memory, int size) : base(memory, size)
        {
            lastAllocatePosition = freeBlocks.First;
        }

        private LinkedListNode<MemoryBlock>? lastAllocatePosition;
    }
}
