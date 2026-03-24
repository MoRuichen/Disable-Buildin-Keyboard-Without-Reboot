using System.Collections.Generic;

namespace SwitchKeyboardTray
{
    public static class DeviceSelectionResolver
    {
        public static KeyboardDeviceInfo Resolve(IEnumerable<KeyboardDeviceInfo> devices, string hardwareId)
        {
            if (devices == null || string.IsNullOrEmpty(hardwareId) || hardwareId.Trim().Length == 0)
            {
                return null;
            }

            foreach (var device in devices)
            {
                if (string.Equals(device.HardwareId, hardwareId, System.StringComparison.OrdinalIgnoreCase))
                {
                    return device;
                }
            }

            return null;
        }
    }
}
