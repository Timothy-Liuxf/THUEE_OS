# lab3

## 简介

动态分区存储管理实现与可视化，包含首次适配、下次适配、最佳适配、最差适配等分配策略。

## 如何使用

### 预先准备

+ .NET 6.0 SDK
+ Linux、Windows、MacOS 来运行测试项目
+ Windows 来可视化

### 快速开始

在 **Windows** 上，运行：

```shell
$ dotnet run -c Release --project ./src/DynamicPartitionedStorageManagement/GUIEntrance/GUIEntrance.csproj
```

然后 GUI 程序就会开始执行

### 如何运行测试

要运行测试项目，请执行：

```shell
$ dotnet test ./src/DynamicPartitionedStorageManagement/UnitTest/UnitTest.csproj
```

然后你会看到所有测试均通过。

### 可视化

要可视化内存使用情况，请在 **Windows** 上执行：

```shell
$ dotnet build -c Release ./src/DynamicPartitionedStorageManagement/DynamicPartitionedStorageManagement.sln
```

然后可执行文件将会生成为：`./src/DynamicPartitionedStorageManagement/GUIEntrance/bin/Release/net6.0-windows/GUIEntrance.exe`。

