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
    internal class FirstFitAllocator : MemoryManager
    {
        protected override nuint? AllocateMemoryImpl(int size)
        {
            if (size <= 0)
            {
                return null;
            }

            for (var freeBlockNode = freeBlocks.First; freeBlockNode is not null; freeBlockNode = freeBlockNode.Next)
            {
                if (freeBlockNode.Value.Size >= size)
                {
                    nuint ans = freeBlockNode.Value.Memory;
                    if (freeBlockNode.Value.Size == size)
                    {
                        freeBlocks.Remove(freeBlockNode);
                    }
                    else
                    {
                        freeBlockNode.ValueRef = new MemoryBlock(freeBlockNode.Value.Memory + (nuint)size, freeBlockNode.Value.Size - size);
                    }
                    return ans;
                }
            }
            return null;
        }

        public FirstFitAllocator(nuint memory, int size) : base(memory, size)
        {

        }
    }
}
