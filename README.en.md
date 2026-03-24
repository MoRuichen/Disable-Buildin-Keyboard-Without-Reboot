[中文](./README.md) | [English](./README.en.md)

# SwitchKeyboardTray

A small Windows tray utility for toggling whether a laptop's built-in keyboard can accept input.

When disabled, it only intercepts the keyboard device you selected. External keyboards still work normally.

## Project Structure

- `src/`: application source code
- `tests/`: test project
- `scripts/`: build, test, and packaging scripts
- `vendor/`: bundled Interception dependency files

## Build And Package

Build the Release version:

```powershell
.\scripts\build.ps1
```

Run tests:

```powershell
.\scripts\test.ps1
```

Package release files into `dist/`:

```powershell
.\scripts\publish.ps1
```

## Release Package Contents

The release zip usually contains these files:

- `SwitchKeyboardTray.exe`
- `interception.dll`
- `install-interception.exe`
- `README.md`
- `README.en.md`

## Usage

### 1. Install The Interception Driver

`install-interception.exe` is a command-line installer. Double-clicking it usually just flashes and exits, which is expected.

Open an elevated terminal in the current folder and run:

```powershell
.\install-interception.exe /install
```

After installation, reboot your computer.

### 2. Start The App

After rebooting, run:

```powershell
SwitchKeyboardTray.exe
```

The app will stay in the system tray.

On first launch, it lists the currently available keyboard devices so you can choose your built-in keyboard.

## Tray Menu

- Enable built-in keyboard input
- Disable built-in keyboard input
- Re-select built-in keyboard
- Exit

## Notes

- When built-in keyboard blocking is enabled, only the selected keyboard device is intercepted.
- External USB or Bluetooth keyboards continue to work normally.
- When the app exits, it automatically restores pass-through behavior so you do not lock yourself out.

## FAQ

### Nothing happens when I double-click `install-interception.exe`

This is normal because it is a command-line installer, not a GUI installer.

Use an elevated terminal instead:

```powershell
.\install-interception.exe /install
```

### The app says Interception is missing

That usually means the driver is not installed correctly, or the machine has not been rebooted yet.

Please confirm:

1. You ran `.\install-interception.exe /install` in an elevated terminal
2. You rebooted the computer
3. `interception.dll` is in the same folder as `SwitchKeyboardTray.exe`

### The device list is hard to understand

The app tries to show Windows keyboard names such as:

- `PS/2 Standard Keyboard`
- `HID Keyboard Device`

If several devices look similar, try one that seems like the built-in keyboard first. If it is wrong, use the re-selection option from the tray menu.

## Config And Logs

The app stores config and logs in:

`%APPDATA%\SwitchKeyboardTray`

## Dependency

This tool depends on the Interception driver.

Official project:

- https://github.com/oblitum/Interception
- https://github.com/oblitum/Interception/releases/tag/v1.0.1
