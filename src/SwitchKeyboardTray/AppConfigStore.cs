using System.IO;
using System.Xml.Serialization;

namespace SwitchKeyboardTray
{
    public class AppConfigStore
    {
        private readonly AppPaths _paths;

        public AppConfigStore(AppPaths paths)
        {
            _paths = paths;
        }

        public AppConfig Load()
        {
            EnsureDirectory();

            if (!File.Exists(_paths.ConfigFilePath))
            {
                return new AppConfig();
            }

            using (var stream = File.OpenRead(_paths.ConfigFilePath))
            {
                var serializer = new XmlSerializer(typeof(AppConfig));
                return (AppConfig)serializer.Deserialize(stream);
            }
        }

        public void Save(AppConfig config)
        {
            EnsureDirectory();

            using (var stream = File.Create(_paths.ConfigFilePath))
            {
                var serializer = new XmlSerializer(typeof(AppConfig));
                serializer.Serialize(stream, config);
            }
        }

        private void EnsureDirectory()
        {
            if (!Directory.Exists(_paths.BaseDirectory))
            {
                Directory.CreateDirectory(_paths.BaseDirectory);
            }
        }
    }
}
