using System;
using System.Windows.Forms;

namespace SwitchKeyboardTray
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var paths = AppPaths.CreateDefault();
            var logger = new FileLogger(paths);
            var configStore = new AppConfigStore(paths);
            var config = configStore.Load();
            var stateController = new StateController(config);
            var interceptionService = new InterceptionKeyboardService(logger);

            Application.Run(new MainApplicationContext(
                configStore,
                config,
                stateController,
                interceptionService,
                logger));
        }
    }
}
