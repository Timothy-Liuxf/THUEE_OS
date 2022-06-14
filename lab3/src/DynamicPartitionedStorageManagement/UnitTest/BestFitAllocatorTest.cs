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
    public class BestFitAllocatorTest
    {
        [TestMethod]
        public void TestAlloction()
        {
            var manager = MemoryManagerFactory.CreateMemoryManager(0u, 1024, MemoryManagerFactory.AllocationStrategy.BestFit);
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
            var manager = MemoryManagerFactory.CreateMemoryManager(0u, 1024, MemoryManagerFactory.AllocationStrategy.BestFit);
            {
                var memory = manager.AllocateMemory(16);
                Assert.IsNotNull(memory);
                Assert.IsTrue(manager.FreeMemory(memory.Value));
            }
        }

        [TestMethod]
        public void TestFailedAllocation()
        {
            var manager = MemoryManagerFactory.CreateMemoryManager(65536u, 1024, MemoryManagerFactory.AllocationStrategy.BestFit);
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
            var manager = MemoryManagerFactory.CreateMemoryManager(128u, 1024, MemoryManagerFactory.AllocationStrategy.BestFit);
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
            var manager = MemoryManagerFactory.CreateMemoryManager(128u, 1024, MemoryManagerFactory.AllocationStrategy.BestFit);
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
            var manager = MemoryManagerFactory.CreateMemoryManager(128u, 1024, MemoryManagerFactory.AllocationStrategy.BestFit);
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
        public void TestBestFit()
        {
            var manager = MemoryManagerFactory.CreateMemoryManager(128u, 1024, MemoryManagerFactory.AllocationStrategy.BestFit);

            {
                // Construct |32| 64 |32| 128 |256| 16 |16| 64 |64| 16 |16| 64 |128| 128

                int[] sizes = { 32, 64, 32, 128, 256, 16, 16, 64, 64, 16, 16, 64, 128, 128 };
                nuint[] memories = new nuint[sizes.Length];
                for (int i = 0; i < sizes.Length; ++i)
                {
                    var tmp = manager.AllocateMemory(sizes[i]);
                    Assert.IsNotNull(tmp);
                    memories[i] = tmp.Value;
                }

                for (int i = 0; i < sizes.Length; i += 2)
                {
                    Assert.IsTrue(manager.FreeMemory(memories[i]));
                }

                {
                    var freeList = manager.GetFreeMemories();
                    Assert.IsTrue(freeList.Count == (sizes.Length + 1) / 2);

                    int i = 0;
                    for (var itr = freeList.First; itr is not null; itr = itr.Next, ++i)
                    {
                        Assert.IsTrue(itr.Value.Size == sizes[i * 2]);
                    }
                }

                var memory1Val = manager.AllocateMemory(96) ?? 0;
                // |32| 64 |32| 128 |256| 16 |16| 64 |64| 16 |16| 64 96 |32| 128
                var memory2Val = manager.AllocateMemory(16) ?? 0;
                // |32| 64 |32| 128 |256| 16 16 64 |64| 16 |16| 64 96 |32| 128
                var memory3Val = manager.AllocateMemory(48) ?? 0;
                // |32| 64 |32| 128 |256| 16 16 64 48 |16| 16 |16| 64 96 |32| 128
                var memory4Val = manager.AllocateMemory(24) ?? 0;
                // 24 |8| 64 |32| 128 |256| 16 16 64 48 |16| 16 |16| 64 96 |32| 128
                var memory5Val = manager.AllocateMemory(0) ?? 0;
                // 24 |8| 64 |32| 128 |256| 16 16 64 48 |16| 16 |16| 64 96 |32| 128
                var memory6Val = manager.AllocateMemory(192) ?? 0;
                // 24 |8| 64 |32| 128 192 |64| 16 16 64 48 |16| 16 |16| 64 96 |32| 128
                var memory7Val = manager.AllocateMemory(16) ?? 0;
                // 24 |8| 64 |32| 128 192 |64| 16 16 64 48 16 16 |16| 64 96 |32| 128
                var memory8Val = manager.AllocateMemory(32) ?? 0;
                // 24 |8| 64 32 128 192 |64| 16 16 64 48 16 16 |16| 64 96 |32| 128
                var memory9Val = manager.AllocateMemory(24) ?? 0;
                // 24 |8| 64 32 128 192 |64| 16 16 64 48 16 16 |16| 64 96 24 |8| 128
                var memory10Val = manager.AllocateMemory(8) ?? 0;
                // 24 8 64 32 128 192 |64| 16 16 64 48 16 16 |16| 64 96 24 |8| 128
                {
                    var freeList = manager.GetFreeMemories();
                    var freeArr = new int[] { 64, 16, 8 };
                    var q = new Queue<int>(freeArr);
                    Assert.IsTrue(freeList.Count == freeArr.Length);
                    foreach (var memoryBlockInfo in freeList)
                    {
                        Assert.IsTrue(memoryBlockInfo.Size == q.Dequeue());
                    }
                }
                manager.FreeMemory(memory4Val);
                manager.FreeMemory(memories[1]);
                manager.FreeMemory(memory10Val);
                // |96| 32 128 192 |64| 16 16 64 48 16 16 |16| 64 96 24 |8| 128
                {
                    var freeList = manager.GetFreeMemories();
                    var freeArr = new int[] { 96, 64, 16, 8 };
                    var q = new Queue<int>(freeArr);
                    Assert.IsTrue(freeList.Count == freeArr.Length);
                    foreach (var memoryBlockInfo in freeList)
                    {
                        Assert.IsTrue(memoryBlockInfo.Size == q.Dequeue());
                    }
                }
                manager.FreeMemory(memories[13]);
                // |96| 32 128 192 |64| 16 16 64 48 16 16 |16| 64 96 24 |136|
                {
                    var freeList = manager.GetFreeMemories();
                    var freeArr = new int[] { 96, 64, 16, 136 };
                    var q = new Queue<int>(freeArr);
                    Assert.IsTrue(freeList.Count == freeArr.Length);
                    foreach (var memoryBlockInfo in freeList)
                    {
                        Assert.IsTrue(memoryBlockInfo.Size == q.Dequeue());
                    }
                }

                for (int i = 1; i < memories.Length; i += 2)
                {
                    manager.FreeMemory(memories[i]);
                }

                // Construct |96| 32 |128| 192 |80| 16 |64| 48 16 |96| 96 24 |136|

                {
                    var freeList = manager.GetFreeMemories();
                    Assert.IsTrue(freeList.Count > 0);
                }

                Assert.IsTrue(memory6Val != 0);
                manager.FreeMemory(memory1Val);
                manager.FreeMemory(memory2Val);
                manager.FreeMemory(memory3Val);
                manager.FreeMemory(memory4Val);
                manager.FreeMemory(memory5Val);
                // manager.FreeMemory(memory6Val);
                manager.FreeMemory(memory7Val);
                manager.FreeMemory(memory8Val);
                manager.FreeMemory(memory9Val);
                manager.FreeMemory(memory10Val);

                {
                    var freeList = manager.GetFreeMemories();
                    Assert.IsTrue(freeList.Count == 2);
                    Assert.IsTrue(freeList.ElementAt(0).Memory == 128u);
                    Assert.IsTrue(freeList.ElementAt(0).Size == 256);
                    Assert.IsTrue(freeList.ElementAt(1).Memory == 576u);
                    Assert.IsTrue(freeList.ElementAt(1).Size == 576);
                }
            }
        }
    }
}
