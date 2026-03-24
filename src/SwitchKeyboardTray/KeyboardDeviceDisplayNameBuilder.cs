using System;
using System.Collections.Generic;

namespace SwitchKeyboardTray
{
    public static class KeyboardDeviceDisplayNameBuilder
    {
        public static string Build(int deviceId, string hardwareId, IDictionary<string, string> friendlyNames)
        {
            var friendlyName = FindFriendlyName(hardwareId, friendlyNames);
            if (!string.IsNullOrEmpty(friendlyName))
            {
                return string.Format("#{0} {1} [{2}]", deviceId, friendlyName, hardwareId);
            }

            return string.Format("#{0} {1}", deviceId, hardwareId);
        }

        public static string FindFriendlyName(string hardwareId, IDictionary<string, string> friendlyNames)
        {
            if (string.IsNullOrEmpty(hardwareId) || friendlyNames == null)
            {
                return null;
            }

            foreach (var pair in friendlyNames)
            {
                if (HardwareIdsMatch(hardwareId, pair.Key))
                {
                    return pair.Value;
                }
            }

            return null;
        }

        public static bool HardwareIdsMatch(string left, string right)
        {
            var normalizedLeft = Normalize(left);
            var normalizedRight = Normalize(right);

            if (string.IsNullOrEmpty(normalizedLeft) || string.IsNullOrEmpty(normalizedRight))
            {
                return false;
            }

            return string.Equals(normalizedLeft, normalizedRight, StringComparison.OrdinalIgnoreCase)
                || normalizedLeft.StartsWith(normalizedRight, StringComparison.OrdinalIgnoreCase)
                || normalizedRight.StartsWith(normalizedLeft, StringComparison.OrdinalIgnoreCase);
        }

        private static string Normalize(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return value.Trim().TrimEnd('\\');
        }
    }
}
