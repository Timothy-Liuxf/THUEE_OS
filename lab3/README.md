# lab3

[English](./README.md)  
[中文（简体）](./README.zh-Hans.md)

## Discription

Dynamic partitioned storage management implementation and visualization, with arrangement strategies such as first fit, next fit, best fit and worst fit.

## How to Use

### Prerequisites

+ .NET 6.0 SDK
+ Linux, Windows or MacOS to run test project
+ Windows to visualize

### Quick Start

On **Windows**, run:

```shell
$ dotnet run -c Release --project ./src/DynamicPartitionedStorageManagement/GUIEntrance/GUIEntrance.csproj
```

And the GUI program will start.

### How to Test

To run test project, please run:

```shell
$ dotnet test ./src/DynamicPartitionedStorageManagement/UnitTest/UnitTest.csproj
```

And you'll see all tests will pass.

### Visualization

To visualize memory usage, you should run:

```shell
$ dotnet build -c Release ./src/DynamicPartitionedStorageManagement/DynamicPartitionedStorageManagement.sln
```

on **Windows**.

And the executable will be `./src/DynamicPartitionedStorageManagement/GUIEntrance/bin/Release/net6.0-windows/GUIEntrance.exe`

