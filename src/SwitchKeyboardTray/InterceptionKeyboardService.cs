using System;
using System.Collections.Generic;
using System.Threading;

namespace SwitchKeyboardTray
{
    public class InterceptionKeyboardService : IKeyboardInterceptionService
    {
        private const ushort FilterAllKeys = 0xFFFF;
        private readonly FileLogger _logger;
        private readonly KeyboardDeviceProvider _provider;
        private readonly object _gate = new object();
        private KeyboardDeviceInfo _selectedKeyboard;
        private bool _blocked;
        private bool _running;
        private Thread _workerThread;
        private IntPtr _context;

        public InterceptionKeyboardService(FileLogger logger)
        {
            _logger = logger;
            _provider = new KeyboardDeviceProvider(logger);
        }

        public bool IsDriverAvailable()
        {
            try
            {
                var context = InterceptionNative.interception_create_context();
                if (context == IntPtr.Zero)
                {
                    _logger.Error("Interception context creation returned null. Driver likely unavailable.");
                    return false;
                }

                InterceptionNative.interception_destroy_context(context);
                return true;
            }
            catch (DllNotFoundException ex)
            {
                _logger.Error("interception.dll was not found.", ex);
                return false;
            }
            catch (BadImageFormatException ex)
            {
                _logger.Error("interception.dll architecture mismatch.", ex);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error("Interception driver availability check failed.", ex);
                return false;
            }
        }

        public IList<KeyboardDeviceInfo> GetKeyboards()
        {
            return _provider.GetKeyboards();
        }

        public void Start()
        {
            lock (_gate)
            {
                if (_running)
                {
                    return;
                }

                _context = InterceptionNative.interception_create_context();
                if (_context == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Unable to create Interception context.");
                }

                InterceptionNative.interception_set_filter(_context, InterceptionNative.KeyboardPredicate, FilterAllKeys);

                _running = true;
                _workerThread = new Thread(WorkerLoop);
                _workerThread.IsBackground = true;
                _workerThread.Name = "InterceptionWorker";
                _workerThread.Start();
                _logger.Info("Interception worker started.");
            }
        }

        public void SetSelectedKeyboard(KeyboardDeviceInfo device)
        {
            lock (_gate)
            {
                _selectedKeyboard = device;
                _logger.Info(device == null
                    ? "Selected keyboard cleared."
                    : "Selected keyboard: " + device.DisplayName);
            }
        }

        public void SetBlocked(bool blocked)
        {
            lock (_gate)
            {
                _blocked = blocked;
                _logger.Info("Built-in keyboard block state changed: " + (_blocked ? "disabled" : "enabled"));
            }
        }

        public void Dispose()
        {
            lock (_gate)
            {
                _blocked = false;
                _running = false;
            }

            if (_workerThread != null && _workerThread.IsAlive)
            {
                _workerThread.Join(1500);
            }

            if (_context != IntPtr.Zero)
            {
                InterceptionNative.interception_destroy_context(_context);
                _context = IntPtr.Zero;
            }

            _logger.Info("Interception worker stopped.");
        }

        private void WorkerLoop()
        {
            while (true)
            {
                KeyboardDeviceInfo selectedKeyboard;
                bool blocked;
                bool running;

                lock (_gate)
                {
                    selectedKeyboard = _selectedKeyboard;
                    blocked = _blocked;
                    running = _running;
                }

                if (!running)
                {
                    return;
                }

                var device = InterceptionNative.interception_wait_with_timeout(_context, 250);
                if (device <= 0)
                {
                    continue;
                }

                if (!InterceptionNative.IsKeyboard(device))
                {
                    continue;
                }

                var stroke = new InterceptionNative.InterceptionKeyStroke();
                var received = InterceptionNative.interception_receive(_context, device, ref stroke, 1);
                if (received <= 0)
                {
                    continue;
                }

                var shouldSwallow = blocked && selectedKeyboard != null && device == selectedKeyboard.DeviceId;
                if (shouldSwallow)
                {
                    continue;
                }

                InterceptionNative.interception_send(_context, device, ref stroke, 1);
            }
        }
    }
}
