using System;
using System.Collections.Generic;

namespace SwitchKeyboardTray
{
    public interface IKeyboardInterceptionService : IDisposable
    {
        bool IsDriverAvailable();

        IList<KeyboardDeviceInfo> GetKeyboards();

        void Start();

        void SetSelectedKeyboard(KeyboardDeviceInfo device);

        void SetBlocked(bool blocked);
    }
}
