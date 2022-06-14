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
    public record struct MemoryBlockInfo(nuint Memory, int Size);

    public partial class MemoryManager
    {
        protected class MemoryBlockComparer : IComparer<MemoryBlock>
        {
            public int Compare(MemoryBlock? x, MemoryBlock? y)
            {
                if (x is null || y is null)
                {
                    throw new ArgumentException("Null Memory Block cannot compare!");
                }

                if (x.Memory < y.Memory)
                {
                    return -1;
                }
                else if (x.Memory == y.Memory)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }

        protected class MemoryBlock
        {
            public MemoryBlock(nuint memory, int size)
            {
                Memory = memory;
                Size = size;
            }

            public nuint Memory { get; private init; }
            public int Size { get; private init; }
        }

    }
}
