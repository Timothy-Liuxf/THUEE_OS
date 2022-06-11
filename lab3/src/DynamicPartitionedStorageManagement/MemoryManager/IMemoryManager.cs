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
