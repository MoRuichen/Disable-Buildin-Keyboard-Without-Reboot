using System;
using System.IO;
using System.Text;

namespace SwitchKeyboardTray
{
    public class FileLogger
    {
        private readonly object _gate = new object();
        private readonly AppPaths _paths;

        public FileLogger(AppPaths paths)
        {
            _paths = paths;
        }

        public void Info(string message)
        {
            Write("INFO", message);
        }

        public void Error(string message)
        {
            Write("ERROR", message);
        }

        public void Error(string message, Exception exception)
        {
            Write("ERROR", message + " | " + exception);
        }

        private void Write(string level, string message)
        {
            lock (_gate)
            {
                if (!Directory.Exists(_paths.BaseDirectory))
                {
                    Directory.CreateDirectory(_paths.BaseDirectory);
                }

                var line = string.Format(
                    "{0:yyyy-MM-dd HH:mm:ss.fff} [{1}] {2}{3}",
                    DateTime.Now,
                    level,
                    message,
                    Environment.NewLine);

                File.AppendAllText(_paths.LogFilePath, line, Encoding.UTF8);
            }
        }
    }
}
