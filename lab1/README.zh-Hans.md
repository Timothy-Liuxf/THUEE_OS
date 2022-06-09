# lab1

## 简介

Linux x86_64 平台上的银行柜员服务问题解答

## 如何使用

### 预先准备

+ GNU Make
+ 支持 C++11 的 C++ 编译器

### 快速开始

要运行本程序，请执行：

```shell
$ make run <env>
```

其中，`<env>` 可以是:

+ `CXX`：指定 C++ 编译器. 默认为 `g++`
+ `CXXFLAGS`：编译选项. 建议使用默认值
+ `NTELLER`：银行柜员个数. 默认为 3
+ `NCUSTOMER`：顾客个数. 默认为 10
+ `MINSERVTIME`：每位顾客服务时间的最小值. 默认为 1
+ `MAXSERVTIME`：每位顾客服务时间的最大值. 默认为 10
+ `MINENTERTIME`：每位顾客进入银行时间的最小值. 默认为 1
+ `MAXENTERTIME`：每位顾客进入银行时间的最大值. 默认为 10

例如：

```shell
$ make run NTELLER=10 NCUSTOMER=20 MAXSERVTIME=5 MAXENTERTIME=40
```

### 构建项目

要构建该项目，请执行：

```shell
$ make
```

或

```shell
$ make build
```

然后程序将会生成在 `src/target` 中.

### 生成测例

要生成测例，请执行：

```shell
$ make test <env>
```

其中，`<env>` 可以是 `NCUSTOMER`, `MINSERVTIME`, `MAXSERVTIME`, `MINENTERTIME` or `MAXENTERTIME`. 含义与之前介绍的相同.

要生成新的测例，请执行：

```shell
$ make retest <env>
```

### 运行测例

要运行在 `make test` 或 `make retest` 中生成的测例，请执行：

```shell
$ make run NTELLER=<value>
```

如果之前没有测例被生成，此命令将会生成一个新的测例. 正如“快速开始”中所展示的.

### 手动输入测例

执行：

```shell
$ make debug NTELLER=<value>
```

或者在构建项目之后执行 `NTELLER=<value> src/target/main`.

然后程序将会等待你的键盘输入. 输入格式为：

+ 多行
+ 每一行包含由空格或制表符分隔的三个整数，表示一个顾客. 第一个整数是该顾客的 ID. 第二个整数是该顾客进入银行的时刻. 第三个整数是该顾客的服务时长

例如：

```
1 1 10
2 5 2
3 6 3
```

### 清理项目

要删除构建项目过程中产生的文件，执行：

```shell
$ make clean
```
