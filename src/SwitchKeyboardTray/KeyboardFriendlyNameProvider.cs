using System;
using System.Collections.Generic;
using System.Management;

namespace SwitchKeyboardTray
{
    public class KeyboardFriendlyNameProvider
    {
        private readonly FileLogger _logger;

        public KeyboardFriendlyNameProvider(FileLogger logger)
        {
            _logger = logger;
        }

        public IDictionary<string, string> GetFriendlyNames()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using (var searcher = new ManagementObjectSearcher(
                    "SELECT Name, Description, PNPDeviceID FROM Win32_PnPEntity WHERE PNPClass = 'Keyboard'"))
                using (var collection = searcher.Get())
                {
                    foreach (ManagementObject item in collection)
                    {
                        var pnpDeviceId = item["PNPDeviceID"] as string;
                        if (string.IsNullOrEmpty(pnpDeviceId))
                        {
                            continue;
                        }

                        var friendlyName = item["Name"] as string;
                        if (string.IsNullOrEmpty(friendlyName))
                        {
                            friendlyName = item["Description"] as string;
                        }

                        if (string.IsNullOrEmpty(friendlyName))
                        {
                            continue;
                        }

                        result[pnpDeviceId] = friendlyName;
                        _logger.Info("Resolved keyboard friendly name: " + friendlyName + " | " + pnpDeviceId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to query keyboard friendly names from WMI.", ex);
            }

            return result;
        }
    }
}
