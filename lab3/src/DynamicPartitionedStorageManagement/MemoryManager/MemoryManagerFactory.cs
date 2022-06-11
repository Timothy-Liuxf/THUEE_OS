namespace MemoryManager
{
    public static class MemoryManagerFactory
    {
        public enum AllocationStrategy
        {
            Default = 0,
            FirstFit = 1,
            NextFit = 2,
            BestFit = 3,
            WorstFit = 4,
        }

        public static MemoryManager CreateMemoryManager(nuint memory, int size, AllocationStrategy allocationStrategy = AllocationStrategy.Default)
        {
            switch (allocationStrategy)
            {
                case AllocationStrategy.Default:
                case AllocationStrategy.FirstFit:
                    return new FirstFitAllocator(memory, size);
                case AllocationStrategy.NextFit:
                    return new NextFitAllocator(memory, size);
                case AllocationStrategy.BestFit:
                    return new BestFitAllocator(memory, size);
                case AllocationStrategy.WorstFit:
                    return new WorstFitAllocator(memory, size);
            }
            throw new ArgumentException("Invalid allocation strategy!");
        }
    }
}
