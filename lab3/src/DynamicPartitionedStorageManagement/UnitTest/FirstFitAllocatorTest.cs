////////////////////////////////////////////////////////////////////////////////
//
// This file is part of the THUEE_OS project.
//
// Copyright (C) 2022 Timothy-LiuXuefeng
//
// MIT License
//

namespace UnitTest
{
    [TestClass]
    public class FirstFitAllocatorTest
    {
        [TestMethod]
        public void TestAlloction()
        {
            var manager = MemoryManagerFactory.CreateMemoryManager(0u, 1024, MemoryManagerFactory.AllocationStrategy.FirstFit);
            Assert.IsNotNull(manager);
            {
                var memory = manager.AllocateMemory(16);
                Assert.IsNotNull(memory);
                Assert.IsTrue(memory.Value >= 0 && memory.Value < 1024);
            }
        }

        [TestMethod]
        public void TestFree()
        {
            var manager = MemoryManagerFactory.CreateMemoryManager(0u, 1024, MemoryManagerFactory.AllocationStrategy.FirstFit);
            {
                var memory = manager.AllocateMemory(16);
                Assert.IsNotNull(memory);
                Assert.IsTrue(manager.FreeMemory(memory.Value));
            }
        }

        [TestMethod]
        public void TestFailedAllocation()
        {
            var manager = MemoryManagerFactory.CreateMemoryManager(65536u, 1024, MemoryManagerFactory.AllocationStrategy.FirstFit);
            {
                Assert.IsNull(manager.AllocateMemory(-1));
                Assert.IsNull(manager.AllocateMemory(2048));
            }

            {
                var memory1 = manager.AllocateMemory(256);
                Assert.IsNotNull(memory1);
                Assert.IsTrue(memory1.Value >= 65536 && memory1.Value < 65536 + 1024);
                var memory2 = manager.AllocateMemory(512);
                Assert.IsNotNull(memory2);
                Assert.IsTrue(memory2.Value >= 65536 && memory2.Value < 65536 + 1024);
                Assert.IsTrue(memory2.Value < memory1.Value || memory2.Value >= memory1.Value + 256);
                var memory3 = manager.AllocateMemory(512);
                Assert.IsNull(memory3);
                Assert.IsTrue(manager.FreeMemory(memory2.Value));
                Assert.IsTrue(manager.FreeMemory(memory1.Value));
            }
        }

        [TestMethod]
        public void TestFailedFree()
        {
            var manager = MemoryManagerFactory.CreateMemoryManager(128u, 1024, MemoryManagerFactory.AllocationStrategy.FirstFit);
            {
                Assert.IsFalse(manager.FreeMemory(0u));
                Assert.IsFalse(manager.FreeMemory(1 << 16));

                var memory1 = manager.AllocateMemory(256);
                Assert.IsNotNull(memory1);
                var memory2 = manager.AllocateMemory(512);
                Assert.IsNotNull(memory2);

                Assert.IsFalse(manager.FreeMemory(memory1.Value + 1));
                Assert.IsFalse(manager.FreeMemory(memory2.Value - 1));

                Assert.IsTrue(manager.FreeMemory(memory2.Value));
                Assert.IsTrue(manager.FreeMemory(memory1.Value));
                Assert.IsFalse(manager.FreeMemory(memory1.Value));
                Assert.IsFalse(manager.FreeMemory(memory2.Value));
            }
        }

        [TestMethod]
        public void TestFreeList()
        {
            var manager = MemoryManagerFactory.CreateMemoryManager(128u, 1024, MemoryManagerFactory.AllocationStrategy.FirstFit);
            {
                var memory1 = manager.AllocateMemory(256);
                Assert.IsNotNull(memory1);
                var memory2 = manager.AllocateMemory(128);
                Assert.IsNotNull(memory2);
                var memory3 = manager.AllocateMemory(128);
                Assert.IsNotNull(memory3);
                var memory4 = manager.AllocateMemory(512);
                Assert.IsNotNull(memory4);

                Assert.IsTrue(manager.GetFreeMemories().Count == 0);
            }
        }

        [TestMethod]
        public void TestMemoryLeak()
        {
            var manager = MemoryManagerFactory.CreateMemoryManager(128u, 1024, MemoryManagerFactory.AllocationStrategy.FirstFit);
            {
                var memory1 = manager.AllocateMemory(256);
                Assert.IsNotNull(memory1);
                var memory2 = manager.AllocateMemory(512);
                Assert.IsNotNull(memory2);
                Assert.IsTrue(manager.FreeMemory(memory2.Value));
                Assert.IsTrue(manager.FreeMemory(memory1.Value));

                var freeList = manager.GetFreeMemories();
                Assert.AreEqual(1, freeList.Count);

                var allBlock = freeList.First?.Value;
                Assert.IsNotNull(allBlock);
                if (allBlock is not null)
                {
                    Assert.IsTrue(allBlock.Value.Memory == 128u);
                    Assert.IsTrue(allBlock.Value.Size == 1024);
                }
            }
        }

        [TestMethod]
        public void TestFirstFit()
        {
            var manager = MemoryManagerFactory.CreateMemoryManager(128u, 1024, MemoryManagerFactory.AllocationStrategy.FirstFit);
            {
                var memoryVal1 = manager.AllocateMemory(256) ?? 0;
                var memoryVal2 = manager.AllocateMemory(128) ?? 0;
                var memoryVal3 = manager.AllocateMemory(128) ?? 0;
                var memoryVal4 = manager.AllocateMemory(512) ?? 0;

                // 256, 128, 128, 512

                Assert.IsTrue(manager.FreeMemory(memoryVal3));
                var memory5 = manager.AllocateMemory(256);
                Assert.IsNull(memory5);

                // 256, 128, |128|, 512

                var memoryVal6 = manager.AllocateMemory(64) ?? 0;
                Assert.IsTrue(manager.FreeMemory(memoryVal2));

                // 256, |128|, 64, |64|, 512

                var memoryVal7 = manager.AllocateMemory(32) ?? 0;

                // 256, 32, |96|, 64, |64|, 512

                Assert.IsTrue(manager.FreeMemory(memoryVal4));

                // 256, 32, |96|, 64, |576|

                var freeBlocks = manager.GetFreeMemories();
                Assert.IsTrue(freeBlocks.Count == 2);
                Assert.IsTrue(freeBlocks.ElementAt(0).Memory == 128u + 256u + 32u);
                Assert.IsTrue(freeBlocks.ElementAt(0).Size == 96);
                Assert.IsTrue(freeBlocks.ElementAt(1).Memory == 128u + 256u + 32u + 96u + 64u);
                Assert.IsTrue(freeBlocks.ElementAt(1).Size == 576);

                Assert.IsTrue(manager.FreeMemory(memoryVal6));

                // 256, 32, |96|, 64, |736|

                Assert.IsTrue(manager.GetFreeMemories().Last?.Value.Size == 736);

                manager.FreeMemory(memoryVal1);
                manager.FreeMemory(memoryVal7);

                Assert.IsTrue(manager.GetFreeMemories().First?.Value.Size == 1024);
            }
        }
    }
}
