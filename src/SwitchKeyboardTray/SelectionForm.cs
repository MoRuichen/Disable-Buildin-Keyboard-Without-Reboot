using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SwitchKeyboardTray
{
    public class SelectionForm : Form
    {
        private readonly ListBox _deviceList;
        private readonly Button _confirmButton;

        public SelectionForm(IList<KeyboardDeviceInfo> devices, KeyboardDeviceInfo selectedDevice)
        {
            Text = "选择内置键盘";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(700, 360);

            var tipLabel = new Label();
            tipLabel.AutoSize = false;
            tipLabel.Location = new Point(12, 12);
            tipLabel.Size = new Size(676, 50);
            tipLabel.Text = "请选择需要作为“内置键盘”的设备。禁用时只会吞掉这把键盘的按键，其他键盘仍然正常工作。";

            _deviceList = new ListBox();
            _deviceList.Location = new Point(12, 72);
            _deviceList.Size = new Size(676, 220);
            _deviceList.DataSource = devices;

            _confirmButton = new Button();
            _confirmButton.Location = new Point(532, 308);
            _confirmButton.Size = new Size(75, 30);
            _confirmButton.Text = "确定";
            _confirmButton.Click += ConfirmButton_Click;

            var cancelButton = new Button();
            cancelButton.Location = new Point(613, 308);
            cancelButton.Size = new Size(75, 30);
            cancelButton.Text = "取消";
            cancelButton.Click += CancelButton_Click;

            Controls.Add(tipLabel);
            Controls.Add(_deviceList);
            Controls.Add(_confirmButton);
            Controls.Add(cancelButton);

            if (selectedDevice != null)
            {
                _deviceList.SelectedItem = selectedDevice;
            }
            else if (_deviceList.Items.Count > 0)
            {
                _deviceList.SelectedIndex = 0;
            }
        }

        public KeyboardDeviceInfo SelectedDevice
        {
            get { return _deviceList.SelectedItem as KeyboardDeviceInfo; }
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            if (SelectedDevice == null)
            {
                MessageBox.Show(this, "请先选择一个键盘设备。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
