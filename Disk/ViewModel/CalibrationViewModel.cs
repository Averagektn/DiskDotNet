using Disk.Data.Impl;
using Disk.Properties.Langs.Calibration;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel;

public class CalibrationViewModel : ObserverViewModel
{
    private string _xCoord;
    public string XCoord { get => _xCoord; set => SetProperty(ref _xCoord, value); }

    private string _yCoord;
    public string YCoord { get => _yCoord; set => SetProperty(ref _yCoord, value); }

    private bool _calibrateXEnabled;
    public bool CalibrateXEnabled { get => _calibrateXEnabled; set => SetProperty(ref _calibrateXEnabled, value); }

    private bool _calibrateYEnabled;
    public bool CalibrateYEnabled { get => _calibrateYEnabled; set => SetProperty(ref _calibrateYEnabled, value); }

    private bool _startCalibrationEnabled = true;
    public bool StartCalibrationEnabled { get => _startCalibrationEnabled; set => SetProperty(ref _startCalibrationEnabled, value); }

    // Actions
    public ICommand StartCalibrationCommand => new Command(StartCalibration);
    public ICommand CentralizeXCommand => new Command(_ => XShift += XAngleRes);
    public ICommand CentralizeYCommand => new Command(_ => YShift += YAngleRes);
    public ICommand ApplyCommand => new Command(ApplyCalibration);
    public ICommand CalibrateXCommand => new Command(_ =>
    {
        CalibrateXEnabled = false;
        IsRunningThread = CalibrateYEnabled;
    });

    public ICommand CalibrateYCommand => new Command(_ =>
    {
        CalibrateYEnabled = false;
        IsRunningThread = CalibrateXEnabled;
    });

    // Non-binded
    private static Settings Settings => Settings.Default;
    private Thread? DataThread;
    private readonly DispatcherTimer TextBoxUpdateTimer;

    private float XAngleRes => XAngle - XShift;
    private float YAngleRes => YAngle - YShift;

    private float XShift = Settings.XAngleShift;
    private float YShift = Settings.YAngleShift;

    private float XAngle = Settings.XMaxAngle;
    private float YAngle = Settings.YMaxAngle;

    private bool IsRunningThread = true;

    public CalibrationViewModel()
    {
        _xCoord = $"{XAngle:F2}";
        _yCoord = $"{YAngle:F2}";

        TextBoxUpdateTimer = new(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(Settings.ShotTime)
        };
        TextBoxUpdateTimer.Tick += UpdateText;
    }

    private void UpdateText(object? sender, EventArgs e)
    {
        if (CalibrateXEnabled)
        {
            XCoord = $"{XAngleRes:F2}";
        }
        if (CalibrateYEnabled)
        {
            YCoord = $"{YAngleRes:F2}";
        }
    }

    private void ReceiveFromDisk()
    {
        try
        {
            using var con = Connection.GetConnection(IPAddress.Parse(Settings.IP), Settings.Port);

            while (IsRunningThread)
            {
                var data = con.GetXYZ();

                if (data is not null)
                {
                    XAngle = data.X;
                    YAngle = data.Y;
                }
            }
        }
        catch
        {
            _ = MessageBox.Show(CalibrationLocalization.ConnectionLost);
        }
    }

    private void StartCalibration(object? parameter)
    {
        StartCalibrationEnabled = false;

        IsRunningThread = true;
        CalibrateXEnabled = true;
        CalibrateYEnabled = true;

        DataThread = new(ReceiveFromDisk);
        DataThread.Start();
        TextBoxUpdateTimer.Start();
    }

    private void ApplyCalibration(object? parameter)
    {
        IsRunningThread = false;

        TextBoxUpdateTimer.Stop();

        if (DataThread is not null && DataThread.IsAlive)
        {
            DataThread.Join();
        }

        Settings.XMaxAngle = Math.Abs(Convert.ToSingle(XCoord));
        Settings.YMaxAngle = Math.Abs(Convert.ToSingle(YCoord));

        Settings.XAngleShift = XShift;
        Settings.YAngleShift = YShift;

        Settings.Save();
        IniNavigationStore.Close();
    }

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);

        IsRunningThread = false;

        TextBoxUpdateTimer.Stop();

        if (DataThread is not null && DataThread.IsAlive)
        {
            DataThread.Join();
        }
    }

    public override void Refresh()
    {
        base.Refresh();

        StartCalibrationEnabled = true;
    }
}