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
    public interface IMemoryManager
    {
        nuint? AllocateMemory(int size);
        bool FreeMemory(nuint memory);
        LinkedList<MemoryBlockInfo> GetFreeMemories();
        LinkedList<MemoryBlockInfo> GetAllocatedMemories();
    }
}
