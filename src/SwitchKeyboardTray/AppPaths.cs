using System;
using System.IO;

namespace SwitchKeyboardTray
{
    public class AppPaths
    {
        public AppPaths(string baseDirectory)
        {
            BaseDirectory = baseDirectory;
            ConfigFilePath = Path.Combine(BaseDirectory, "config.xml");
            LogFilePath = Path.Combine(BaseDirectory, "app.log");
        }

        public string BaseDirectory { get; private set; }

        public string ConfigFilePath { get; private set; }

        public string LogFilePath { get; private set; }

        public static AppPaths CreateDefault()
        {
            var baseDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SwitchKeyboardTray");

            return new AppPaths(baseDirectory);
        }
    }
}
