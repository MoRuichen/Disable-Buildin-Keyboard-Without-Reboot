using System.Collections.Generic;

namespace SwitchKeyboardTray
{
    public class AppCoordinator
    {
        private readonly AppConfigStore _configStore;
        private readonly AppConfig _config;
        private readonly StateController _stateController;
        private readonly IKeyboardInterceptionService _interceptionService;
        private readonly FileLogger _logger;
        private KeyboardDeviceInfo _selectedKeyboard;

        public AppCoordinator(
            AppConfigStore configStore,
            AppConfig config,
            StateController stateController,
            IKeyboardInterceptionService interceptionService,
            FileLogger logger)
        {
            _configStore = configStore;
            _config = config;
            _stateController = stateController;
            _interceptionService = interceptionService;
            _logger = logger;
        }

        public KeyboardDeviceInfo SelectedKeyboard
        {
            get { return _selectedKeyboard; }
        }

        public bool IsBlocked
        {
            get { return _stateController.IsBlocked; }
        }

        public IList<KeyboardDeviceInfo> GetKeyboards()
        {
            return _interceptionService.GetKeyboards();
        }

        public KeyboardDeviceInfo ResolveConfiguredSelection(IList<KeyboardDeviceInfo> keyboards)
        {
            _selectedKeyboard = DeviceSelectionResolver.Resolve(keyboards, _config.SelectedHardwareId);
            if (_selectedKeyboard != null)
            {
                _logger.Info("Resolved configured keyboard: " + _selectedKeyboard.DisplayName);
            }

            return _selectedKeyboard;
        }

        public void ApplySelection(KeyboardDeviceInfo device)
        {
            _selectedKeyboard = device;
            _stateController.SelectKeyboard(device);
            _configStore.Save(_config);
            _interceptionService.SetSelectedKeyboard(device);
            _logger.Info(device == null
                ? "Keyboard selection cleared."
                : "Keyboard selected: " + device.DisplayName);
        }

        public void Start()
        {
            _interceptionService.Start();
            _interceptionService.SetSelectedKeyboard(_selectedKeyboard);
            _interceptionService.SetBlocked(_stateController.IsBlocked);
        }

        public void Enable()
        {
            _stateController.EnableSelectedKeyboard();
            _configStore.Save(_config);
            _interceptionService.SetBlocked(false);
            _logger.Info("Built-in keyboard input enabled.");
        }

        public void Disable()
        {
            _stateController.DisableSelectedKeyboard();
            _configStore.Save(_config);
            _interceptionService.SetSelectedKeyboard(_selectedKeyboard);
            _interceptionService.SetBlocked(true);
            _logger.Info("Built-in keyboard input disabled.");
        }

        public void Shutdown()
        {
            _interceptionService.SetBlocked(false);
            _interceptionService.Dispose();
            _logger.Info("Interception shutdown complete.");
        }
    }
}
