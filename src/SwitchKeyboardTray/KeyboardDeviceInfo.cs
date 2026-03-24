namespace SwitchKeyboardTray
{
    public class KeyboardDeviceInfo
    {
        public int DeviceId { get; set; }

        public string HardwareId { get; set; }

        public string DisplayName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
