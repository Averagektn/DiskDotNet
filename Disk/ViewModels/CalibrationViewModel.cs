using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Disk.Data.Impl;
using Disk.Navigators;
using Disk.Stores;
using Disk.ViewModels.Common.Commands.Sync;
using Disk.ViewModels.Common.ViewModels;

using Serilog;

using Localization = Disk.Properties.Langs.Calibration.CalibrationLocalization;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModels;

public class CalibrationViewModel : PopupViewModel
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

    private readonly ModalNavigationStore _modalNavigationStore;

    public CalibrationViewModel(ModalNavigationStore modalNavigationStore)
    {
        _modalNavigationStore = modalNavigationStore;

        _xCoord = $"{XAngle:F2}";
        _yCoord = $"{YAngle:F2}";

        TextBoxUpdateTimer = new(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(Settings.ShotTime)
        };
        TextBoxUpdateTimer.Tick += UpdateText;
    }

    public ICommand StartCalibrationCommand => new Command(_ =>
    {
        StartCalibrationEnabled = false;

        IsRunningThread = true;
        CalibrateXEnabled = true;
        CalibrateYEnabled = true;

        DataThread = new(ReceiveFromDisk);
        DataThread.Start();
        TextBoxUpdateTimer.Start();
    });

    public ICommand CentralizeXCommand => new Command(_ => XShift += XAngleRes);

    public ICommand CentralizeYCommand => new Command(_ => YShift += YAngleRes);

    public ICommand ApplyCommand => new Command(_ =>
    {
        SaveSettings();
        IniNavigationStore.Close();

        Log.Information("Calibration applied");
    });

    public ICommand CalibrateXCommand => new Command(_ =>
    {
        CalibrateXEnabled = false;
        IsRunningThread = CalibrateYEnabled;
        Log.Information("Calibrate X");
    });

    public ICommand CalibrateYCommand => new Command(_ =>
    {
        CalibrateYEnabled = false;
        IsRunningThread = CalibrateXEnabled;
        Log.Information("Calibrate Y");
    });

    private void SaveSettings()
    {
        IsRunningThread = false;

        TextBoxUpdateTimer.Stop();

        if (DataThread is not null && DataThread.IsAlive)
        {
            DataThread.Join();
        }

        XAngle = Math.Abs(Convert.ToSingle(XCoord));
        YAngle = Math.Abs(Convert.ToSingle(YCoord));

        Settings.XMaxAngle = XAngle;
        Settings.YMaxAngle = YAngle;

        Settings.XAngleShift = XShift;
        Settings.YAngleShift = YShift;

        Settings.Save();
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
            Log.Information("Calibration coordiantes receiving");
            using var con = Connection.GetConnection(IPAddress.Parse(Settings.IP), Settings.Port);

            while (IsRunningThread)
            {
                Point3D<float>? data = con.GetXYZ();

                if (data is not null)
                {
                    XAngle = data.X;
                    YAngle = data.Y;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Calibration failed: {ex.Message} {ex.StackTrace}");
            _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                await ShowPopup(header: Localization.ConnectionLost, message: ""));
            IsRunningThread = false;
            StartCalibrationEnabled = true;
        }
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

        IsRunningThread = false;

        TextBoxUpdateTimer.Stop();

        if (DataThread is not null && DataThread.IsAlive)
        {
            DataThread.Join();
        }

        StartCalibrationEnabled = true;
        XCoord = $"{Settings.XMaxAngle:f2}";
        YCoord = $"{Settings.YMaxAngle:f2}";
    }

    public override void AfterNavigation()
    {
        base.AfterNavigation();

        XAngle = Math.Abs(Convert.ToSingle(XCoord));
        YAngle = Math.Abs(Convert.ToSingle(YCoord));

        bool xAngleChanged = float.Abs(XAngle - Settings.XMaxAngle) >= 0.01;
        bool yAngleChanged = float.Abs(YAngle - Settings.YMaxAngle) >= 0.01;
        bool xShiftChanged = float.Abs(XShift - Settings.XAngleShift) >= 0.01;
        bool yShiftChanged = float.Abs(YShift - Settings.YAngleShift) >= 0.01;

        if (xAngleChanged || yAngleChanged || xShiftChanged || yShiftChanged)
        {
            if (_modalNavigationStore.CurrentViewModel is not QuestionViewModel)
            {
                QuestionNavigator.Navigate(IniNavigationStore.CurrentViewModel ?? this, _modalNavigationStore,
                    message: Localization.UnsavedCalibration,
                    beforeConfirm: SaveSettings);
            }
        }
    }
}