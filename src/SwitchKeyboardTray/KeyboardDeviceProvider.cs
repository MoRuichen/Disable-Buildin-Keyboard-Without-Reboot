using System;
using System.Collections.Generic;
using System.Text;

namespace SwitchKeyboardTray
{
    public class KeyboardDeviceProvider
    {
        private const int MaxKeyboardDevices = 10;
        private readonly FileLogger _logger;
        private readonly KeyboardFriendlyNameProvider _friendlyNameProvider;

        public KeyboardDeviceProvider(FileLogger logger)
        {
            _logger = logger;
            _friendlyNameProvider = new KeyboardFriendlyNameProvider(logger);
        }

        public IList<KeyboardDeviceInfo> GetKeyboards()
        {
            var devices = new List<KeyboardDeviceInfo>();
            var context = IntPtr.Zero;

            try
            {
                var friendlyNames = _friendlyNameProvider.GetFriendlyNames();
                context = InterceptionNative.interception_create_context();
                if (context == IntPtr.Zero)
                {
                    _logger.Error("Interception context creation failed while enumerating keyboards.");
                    return devices;
                }

                for (var deviceId = 1; deviceId <= MaxKeyboardDevices; deviceId++)
                {
                    if (!InterceptionNative.IsKeyboard(deviceId) || InterceptionNative.IsInvalid(deviceId))
                    {
                        continue;
                    }

                    var buffer = new StringBuilder(1024);
                    var length = InterceptionNative.interception_get_hardware_id(context, deviceId, buffer, 1024);
                    if (length == 0)
                    {
                        continue;
                    }

                    var hardwareId = buffer.ToString();
                    var displayName = KeyboardDeviceDisplayNameBuilder.Build(deviceId, hardwareId, friendlyNames);
                    var device = new KeyboardDeviceInfo
                    {
                        DeviceId = deviceId,
                        HardwareId = hardwareId,
                        DisplayName = displayName
                    };

                    _logger.Info("Enumerated keyboard: " + displayName);
                    devices.Add(device);
                }
            }
            catch (DllNotFoundException ex)
            {
                _logger.Error("interception.dll was not found during keyboard enumeration.", ex);
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error during keyboard enumeration.", ex);
            }
            finally
            {
                if (context != IntPtr.Zero)
                {
                    InterceptionNative.interception_destroy_context(context);
                }
            }

            return devices;
        }
    }
}
