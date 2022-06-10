namespace MemoryManager
{
    public record struct MemoryBlockInfo(nuint Memory, int size);

    public partial class MemoryManager
    {
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
