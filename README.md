[中文](./README.md) | [English](./README.en.md)

# SwitchKeyboardTray

一个 Windows 托盘小工具，用来切换“笔记本内置键盘是否可输入”。

禁用时，只会拦截你选中的那把键盘，其他外接键盘仍然可以正常输入。

## 项目结构

- `src/`：主程序源码
- `tests/`：测试项目
- `scripts/`：构建、测试、打包脚本
- `vendor/`：Interception 相关依赖文件

## 开发和打包

构建 Release：

```powershell
.\scripts\build.ps1
```

运行测试：

```powershell
.\scripts\test.ps1
```

打包发布文件到 `dist/`：

```powershell
.\scripts\publish.ps1
```

## 发布包内容

Release 压缩包通常包含这些文件：

- `SwitchKeyboardTray.exe`
- `interception.dll`
- `install-interception.exe`
- `README.md`
- `README.en.md`

## 使用方法

### 1. 安装 Interception 驱动

`install-interception.exe` 不是图形安装器，直接双击通常只会一闪而过，这是正常现象。

请用“管理员权限”的终端进入当前目录后执行：

```powershell
.\install-interception.exe /install
```

安装完成后，重启电脑。

### 2. 启动程序

重启后，双击运行：

```powershell
SwitchKeyboardTray.exe
```

程序启动后会常驻系统托盘。

第一次运行时，会列出当前键盘设备，请选择你的“内置键盘”。

## 托盘菜单

- 启用内置键盘输入
- 禁用内置键盘输入
- 重新选择内置键盘
- 退出

## 使用说明

- “禁用内置键盘输入”开启后，只会吞掉所选键盘的按键事件。
- 外接 USB 键盘、蓝牙键盘等其他键盘仍然可以正常使用。
- 程序退出时会自动恢复放行，避免把自己锁死。

## 常见问题

### 双击 `install-interception.exe` 没反应

这是正常的，因为它是命令行安装工具，不是图形安装器。

请改用管理员终端执行：

```powershell
.\install-interception.exe /install
```

### 运行程序后提示找不到 Interception

说明驱动还没装好，或者电脑还没有重启。

请确认：

1. 已用管理员权限执行 `.\install-interception.exe /install`
2. 已重启电脑
3. `interception.dll` 与 `SwitchKeyboardTray.exe` 在同一目录

### 选设备时看不懂列表

程序会尽量显示 Windows 里的键盘名称，例如：

- `PS/2 标准键盘`
- `HID Keyboard Device`

如果有多个相似设备，可以先选内置键盘试一下；不对的话，再用“重新选择内置键盘”切换。

## 配置和日志

程序会把配置和日志写到：

`%APPDATA%\SwitchKeyboardTray`

## 依赖说明

本工具依赖 Interception 驱动。

官方项目：

- https://github.com/oblitum/Interception
- https://github.com/oblitum/Interception/releases/tag/v1.0.1
