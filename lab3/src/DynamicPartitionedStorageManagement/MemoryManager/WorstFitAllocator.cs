////////////////////////////////////////////////////////////////////////////////
//
// This file is part of the THUEE_OS project.
//
// Copyright (C) 2022 Timothy-LiuXuefeng
//
// MIT License
//

namespace MemoryManager
{
    internal class WorstFitAllocator : MemoryManager
    {
        protected override nuint? AllocateMemoryImpl(int size)
        {
            if (size <= 0)
            {
                return null;
            }

            LinkedListNode<MemoryBlock>? result = null;
            for (var freeBlockNode = freeBlocks.First; freeBlockNode is not null; freeBlockNode = freeBlockNode.Next)
            {
                int blockSize = freeBlockNode.Value.Size;
                if (blockSize >= size && (result is null || blockSize > result.Value.Size))
                {
                    result = freeBlockNode;
                }
            }

            if (result is not null)
            {
                nuint ans = result.Value.Memory;
                if (result.Value.Size == size)
                {
                    freeBlocks.Remove(result);
                }
                else
                {
                    result.ValueRef = new MemoryBlock(result.Value.Memory + (nuint)size, result.Value.Size - size);
                }
                return ans;
            }
            return null;
        }

        public WorstFitAllocator(nuint memory, int size) : base(memory, size)
        {

        }
    }
}
