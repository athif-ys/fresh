using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ChargeGuard;

public class MainForm : Form
{
    private readonly NumericUpDown _thresholdInput;
    private readonly NumericUpDown _pollIntervalInput;
    private readonly CheckBox _requireChargingCheck;
    private readonly TextBox _commandInput;
    private readonly Label _statusLabel;
    private readonly Timer _timer;
    private readonly NotifyIcon _notifyIcon;

    private bool _alreadyTriggered;

    public MainForm()
    {
        Text = "ChargeGuard";
        ClientSize = new Size(520, 310);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var thresholdLabel = new Label
        {
            Text = "Stop target (%):",
            Location = new Point(20, 25),
            AutoSize = true
        };
        Controls.Add(thresholdLabel);

        _thresholdInput = new NumericUpDown
        {
            Location = new Point(150, 20),
            Minimum = 50,
            Maximum = 100,
            Value = 80,
            Width = 80
        };
        Controls.Add(_thresholdInput);

        var pollLabel = new Label
        {
            Text = "Poll interval (seconds):",
            Location = new Point(20, 65),
            AutoSize = true
        };
        Controls.Add(pollLabel);

        _pollIntervalInput = new NumericUpDown
        {
            Location = new Point(190, 60),
            Minimum = 5,
            Maximum = 300,
            Value = 15,
            Width = 80
        };
        Controls.Add(_pollIntervalInput);

        _requireChargingCheck = new CheckBox
        {
            Text = "Trigger only when charging",
            Location = new Point(20, 100),
            Width = 250,
            Checked = true
        };
        Controls.Add(_requireChargingCheck);

        var commandLabel = new Label
        {
            Text = "Optional command to run at target (OEM tool call):",
            Location = new Point(20, 140),
            AutoSize = true
        };
        Controls.Add(commandLabel);

        _commandInput = new TextBox
        {
            Location = new Point(20, 165),
            Width = 470,
            PlaceholderText = "Example: powershell -File C:\\Scripts\\set-threshold.ps1"
        };
        Controls.Add(_commandInput);

        var startButton = new Button
        {
            Text = "Start Monitoring",
            Location = new Point(20, 210),
            Width = 150
        };
        startButton.Click += (_, _) => StartMonitoring();
        Controls.Add(startButton);

        var stopButton = new Button
        {
            Text = "Stop",
            Location = new Point(180, 210),
            Width = 100
        };
        stopButton.Click += (_, _) => StopMonitoring("Monitoring stopped.");
        Controls.Add(stopButton);

        _statusLabel = new Label
        {
            Text = "Status: idle",
            Location = new Point(20, 250),
            AutoSize = true
        };
        Controls.Add(_statusLabel);

        _timer = new Timer();
        _timer.Tick += (_, _) => CheckBattery();

        _notifyIcon = new NotifyIcon
        {
            Visible = true,
            Icon = SystemIcons.Information,
            Text = "ChargeGuard"
        };

        FormClosing += (_, _) => _notifyIcon.Dispose();
    }

    private void StartMonitoring()
    {
        _alreadyTriggered = false;
        _timer.Interval = (int)_pollIntervalInput.Value * 1000;
        _timer.Start();
        _statusLabel.Text = $"Status: monitoring every {_pollIntervalInput.Value}s";
        CheckBattery();
    }

    private void StopMonitoring(string message)
    {
        _timer.Stop();
        _alreadyTriggered = false;
        _statusLabel.Text = $"Status: {message}";
    }

    private void CheckBattery()
    {
        var status = SystemInformation.PowerStatus;
        var percent = (int)Math.Round(status.BatteryLifePercent * 100);
        var charging = status.PowerLineStatus == PowerLineStatus.Online;

        _statusLabel.Text = $"Status: {percent}% ({(charging ? "charging" : "not charging")})";

        if (_requireChargingCheck.Checked && !charging)
        {
            _alreadyTriggered = false;
            return;
        }

        if (percent < (int)_thresholdInput.Value)
        {
            _alreadyTriggered = false;
            return;
        }

        if (_alreadyTriggered)
        {
            return;
        }

        _alreadyTriggered = true;
        _notifyIcon.BalloonTipTitle = "Charge target reached";
        _notifyIcon.BalloonTipText = $"Battery reached {percent}%. Unplug charger now.";
        _notifyIcon.ShowBalloonTip(5000);

        var command = _commandInput.Text.Trim();
        if (!string.IsNullOrWhiteSpace(command))
        {
            TryRunCommand(command);
        }

        MessageBox.Show(
            $"Battery reached {percent}%.\n\nIf your device supports it, use the optional OEM command to enforce thresholds.",
            "ChargeGuard",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void TryRunCommand(string command)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            process?.WaitForExit(10000);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to run command:\n{ex.Message}",
                "Command Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}
