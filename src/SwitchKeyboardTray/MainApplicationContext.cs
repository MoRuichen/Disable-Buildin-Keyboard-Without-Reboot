using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SwitchKeyboardTray
{
    public class MainApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ToolStripMenuItem _enableItem;
        private readonly ToolStripMenuItem _disableItem;
        private readonly ToolStripMenuItem _reselectItem;
        private readonly IKeyboardInterceptionService _interceptionService;
        private readonly FileLogger _logger;
        private readonly AppCoordinator _coordinator;

        public MainApplicationContext(
            AppConfigStore configStore,
            AppConfig config,
            StateController stateController,
            IKeyboardInterceptionService interceptionService,
            FileLogger logger)
        {
            _interceptionService = interceptionService;
            _logger = logger;
            _coordinator = new AppCoordinator(configStore, config, stateController, interceptionService, logger);

            var menu = new ContextMenuStrip();
            _enableItem = new ToolStripMenuItem("启用内置键盘输入", null, EnableSelectedKeyboard);
            _disableItem = new ToolStripMenuItem("禁用内置键盘输入", null, DisableSelectedKeyboard);
            _reselectItem = new ToolStripMenuItem("重新选择内置键盘", null, ReselectKeyboard);
            var exitItem = new ToolStripMenuItem("退出", null, ExitApplication);

            menu.Items.Add(_enableItem);
            menu.Items.Add(_disableItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(_reselectItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(exitItem);

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Text = "SwitchKeyboardTray";
            _notifyIcon.Icon = SystemIcons.Application;
            _notifyIcon.ContextMenuStrip = menu;
            _notifyIcon.Visible = true;

            Initialize();
        }

        private void Initialize()
        {
            try
            {
                _logger.Info("Application starting.");

                if (!_interceptionService.IsDriverAvailable())
                {
                    MessageBox.Show(
                        "未检测到可用的 Interception 运行环境。请先安装官方驱动，并将 interception.dll 放到程序目录后再启动。",
                        "Interception 未就绪",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    ExitThread();
                    return;
                }

                if (!TryResolveSelectedKeyboard(true))
                {
                    ExitThread();
                    return;
                }

                _coordinator.Start();
                UpdateMenuState();

                ShowBalloon("SwitchKeyboardTray 已启动。");
            }
            catch (Exception ex)
            {
                _logger.Error("Application initialization failed.", ex);
                MessageBox.Show("程序启动失败，请查看日志。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitThread();
            }
        }

        private bool TryResolveSelectedKeyboard(bool allowPrompt)
        {
            IList<KeyboardDeviceInfo> keyboards = _coordinator.GetKeyboards();
            if (keyboards.Count == 0)
            {
                MessageBox.Show("未枚举到任何键盘设备。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _logger.Info("No keyboards were enumerated.");
                return false;
            }

            if (_coordinator.ResolveConfiguredSelection(keyboards) != null)
            {
                return true;
            }

            if (!allowPrompt)
            {
                return false;
            }

            return PromptForKeyboardSelection(keyboards);
        }

        private bool PromptForKeyboardSelection(IList<KeyboardDeviceInfo> keyboards)
        {
            using (var form = new SelectionForm(keyboards, _coordinator.SelectedKeyboard))
            {
                if (form.ShowDialog() != DialogResult.OK || form.SelectedDevice == null)
                {
                    _logger.Info("Keyboard selection cancelled by user.");
                    return false;
                }

                _coordinator.ApplySelection(form.SelectedDevice);
                return true;
            }
        }

        private void EnableSelectedKeyboard(object sender, EventArgs e)
        {
            _coordinator.Enable();
            UpdateMenuState();
            ShowBalloon("已启用内置键盘输入。");
        }

        private void DisableSelectedKeyboard(object sender, EventArgs e)
        {
            if (_coordinator.SelectedKeyboard == null && !TryResolveSelectedKeyboard(true))
            {
                return;
            }

            _coordinator.Disable();
            UpdateMenuState();
            ShowBalloon("已禁用所选内置键盘输入。");
        }

        private void ReselectKeyboard(object sender, EventArgs e)
        {
            var keyboards = _coordinator.GetKeyboards();
            if (keyboards.Count == 0)
            {
                MessageBox.Show("未枚举到任何键盘设备。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!PromptForKeyboardSelection(keyboards))
            {
                return;
            }

            _logger.Info("User reselected built-in keyboard.");
            ShowBalloon("已更新内置键盘选择。");
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            ExitThread();
        }

        protected override void ExitThreadCore()
        {
            _logger.Info("Application exiting. Restoring pass-through.");
            _coordinator.Shutdown();
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            base.ExitThreadCore();
        }

        private void UpdateMenuState()
        {
            _enableItem.Enabled = _coordinator.IsBlocked;
            _disableItem.Enabled = !_coordinator.IsBlocked;
            _reselectItem.Enabled = true;
        }

        private void ShowBalloon(string text)
        {
            _notifyIcon.BalloonTipTitle = "SwitchKeyboardTray";
            _notifyIcon.BalloonTipText = text;
            _notifyIcon.ShowBalloonTip(2000);
        }
    }
}
