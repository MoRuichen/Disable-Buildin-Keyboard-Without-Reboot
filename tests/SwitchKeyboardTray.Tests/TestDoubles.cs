using System;
using System.Collections.Generic;
using SwitchKeyboardTray;

namespace SwitchKeyboardTray.Tests
{
    internal class FakeKeyboardInterceptionService : IKeyboardInterceptionService
    {
        public bool DriverAvailable = true;
        public bool Started;
        public bool Disposed;
        public bool? LastBlocked;
        public KeyboardDeviceInfo LastSelected;
        public IList<KeyboardDeviceInfo> Keyboards = new List<KeyboardDeviceInfo>();

        public bool IsDriverAvailable()
        {
            return DriverAvailable;
        }

        public IList<KeyboardDeviceInfo> GetKeyboards()
        {
            return Keyboards;
        }

        public void Start()
        {
            Started = true;
        }

        public void SetSelectedKeyboard(KeyboardDeviceInfo device)
        {
            LastSelected = device;
        }

        public void SetBlocked(bool blocked)
        {
            LastBlocked = blocked;
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }
}
