namespace SwitchKeyboardTray
{
    public class StateController
    {
        private readonly AppConfig _config;

        public StateController(AppConfig config)
        {
            _config = config;
        }

        public bool IsBlocked
        {
            get { return _config.BlockBuiltInKeyboard; }
        }

        public void EnableSelectedKeyboard()
        {
            _config.BlockBuiltInKeyboard = false;
        }

        public void DisableSelectedKeyboard()
        {
            _config.BlockBuiltInKeyboard = true;
        }

        public void SelectKeyboard(KeyboardDeviceInfo device)
        {
            _config.SelectedHardwareId = device == null ? null : device.HardwareId;
        }
    }
}
