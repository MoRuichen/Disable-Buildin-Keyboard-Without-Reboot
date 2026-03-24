using System;
using System.Collections.Generic;
using System.IO;
using SwitchKeyboardTray;

namespace SwitchKeyboardTray.Tests
{
    internal static class Tests
    {
        public static void RunAll()
        {
            ConfigRoundTrips();
            DeviceSelectionMatchesHardwareId();
            DisplayNameUsesFriendlyNameWhenMatched();
            DisplayNameFallsBackToHardwareIdWhenUnknown();
            StateControllerUpdatesConfig();
            CoordinatorRestoresSavedKeyboardAndStartsWithCurrentState();
            CoordinatorEnableDisableAndShutdownSyncService();
        }

        private static void ConfigRoundTrips()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "SwitchKeyboardTrayTests_" + Guid.NewGuid().ToString("N"));
            var store = new AppConfigStore(new AppPaths(tempDir));
            var config = new AppConfig
            {
                SelectedHardwareId = "HID\\TEST",
                BlockBuiltInKeyboard = true
            };

            store.Save(config);
            var loaded = store.Load();

            AssertEqual("HID\\TEST", loaded.SelectedHardwareId, "Config selected hardware id should round-trip.");
            AssertEqual(true, loaded.BlockBuiltInKeyboard, "Config blocked state should round-trip.");
        }

        private static void DeviceSelectionMatchesHardwareId()
        {
            var devices = new List<KeyboardDeviceInfo>
            {
                new KeyboardDeviceInfo { DeviceId = 1, HardwareId = "HID\\A", DisplayName = "A" },
                new KeyboardDeviceInfo { DeviceId = 2, HardwareId = "HID\\B", DisplayName = "B" }
            };

            var selected = DeviceSelectionResolver.Resolve(devices, "hid\\b");

            AssertEqual(2, selected.DeviceId, "Resolver should match hardware ids case-insensitively.");
        }

        private static void StateControllerUpdatesConfig()
        {
            var config = new AppConfig();
            var controller = new StateController(config);
            var device = new KeyboardDeviceInfo { HardwareId = "HID\\INTERNAL" };

            controller.SelectKeyboard(device);
            controller.DisableSelectedKeyboard();
            AssertEqual("HID\\INTERNAL", config.SelectedHardwareId, "Selected keyboard should be stored.");
            AssertEqual(true, controller.IsBlocked, "Controller should reflect blocked state.");

            controller.EnableSelectedKeyboard();
            AssertEqual(false, controller.IsBlocked, "Controller should clear blocked state.");
        }

        private static void DisplayNameUsesFriendlyNameWhenMatched()
        {
            var names = new Dictionary<string, string>
            {
                { "ACPI\\HPQ8001\\4&262F6ADF&0", "PS/2 Standard Keyboard" }
            };

            var displayName = KeyboardDeviceDisplayNameBuilder.Build(
                3,
                "ACPI\\HPQ8001\\4&262F6ADF&0",
                names);

            AssertEqual(
                "#3 PS/2 Standard Keyboard [ACPI\\HPQ8001\\4&262F6ADF&0]",
                displayName,
                "Display name should include a friendly keyboard name when available.");
        }

        private static void DisplayNameFallsBackToHardwareIdWhenUnknown()
        {
            var displayName = KeyboardDeviceDisplayNameBuilder.Build(
                2,
                "HID\\UNKNOWN\\123",
                new Dictionary<string, string>());

            AssertEqual(
                "#2 HID\\UNKNOWN\\123",
                displayName,
                "Display name should fall back to hardware id when no friendly name is available.");
        }

        private static void CoordinatorRestoresSavedKeyboardAndStartsWithCurrentState()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "SwitchKeyboardTrayTests_" + Guid.NewGuid().ToString("N"));
            var paths = new AppPaths(tempDir);
            var store = new AppConfigStore(paths);
            var config = new AppConfig
            {
                SelectedHardwareId = "HID\\BUILTIN",
                BlockBuiltInKeyboard = true
            };
            store.Save(config);

            var logger = new FileLogger(paths);
            var service = new FakeKeyboardInterceptionService();
            service.Keyboards = new List<KeyboardDeviceInfo>
            {
                new KeyboardDeviceInfo { DeviceId = 3, HardwareId = "HID\\BUILTIN", DisplayName = "Builtin" }
            };

            var coordinator = new AppCoordinator(store, config, new StateController(config), service, logger);
            var selected = coordinator.ResolveConfiguredSelection(service.Keyboards);
            coordinator.Start();

            AssertEqual("HID\\BUILTIN", selected.HardwareId, "Coordinator should restore saved keyboard.");
            AssertEqual(true, service.Started, "Coordinator should start interception service.");
            AssertEqual(true, service.LastBlocked, "Coordinator should apply blocked state on start.");
            AssertEqual("HID\\BUILTIN", service.LastSelected.HardwareId, "Coordinator should forward selected keyboard to service.");
        }

        private static void CoordinatorEnableDisableAndShutdownSyncService()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "SwitchKeyboardTrayTests_" + Guid.NewGuid().ToString("N"));
            var paths = new AppPaths(tempDir);
            var store = new AppConfigStore(paths);
            var config = new AppConfig();
            var logger = new FileLogger(paths);
            var service = new FakeKeyboardInterceptionService();
            var coordinator = new AppCoordinator(store, config, new StateController(config), service, logger);
            var device = new KeyboardDeviceInfo { DeviceId = 5, HardwareId = "HID\\BUILTIN", DisplayName = "Builtin" };

            coordinator.ApplySelection(device);
            coordinator.Disable();
            AssertEqual(true, config.BlockBuiltInKeyboard, "Disable should persist blocked state.");
            AssertEqual(true, service.LastBlocked, "Disable should notify service to block keyboard.");
            AssertEqual("HID\\BUILTIN", service.LastSelected.HardwareId, "Disable should preserve selected keyboard.");

            coordinator.Enable();
            AssertEqual(false, config.BlockBuiltInKeyboard, "Enable should persist unblocked state.");
            AssertEqual(false, service.LastBlocked, "Enable should notify service to unblock keyboard.");

            coordinator.Shutdown();
            AssertEqual(false, service.LastBlocked, "Shutdown should restore pass-through before disposal.");
            AssertEqual(true, service.Disposed, "Shutdown should dispose service.");
        }

        private static void AssertEqual<T>(T expected, T actual, string message)
        {
            if (!object.Equals(expected, actual))
            {
                throw new InvalidOperationException(message + " Expected: " + expected + " Actual: " + actual);
            }
        }
    }
}
