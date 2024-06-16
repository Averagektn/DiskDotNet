using Disk.Data.Impl;
using Disk.Properties.Langs.Calibration;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class CalibrationViewModel : ObserverViewModel
    {
        public string XCoord { get => _xCoord; set => SetProperty(ref _xCoord, value); }
        public string YCoord { get => _yCoord; set => SetProperty(ref _yCoord, value); }
        public bool CalibrateXEnabled { get => _calibrateXEnabled; set => SetProperty(ref _calibrateXEnabled, value); }
        public bool CalibrateYEnabled { get => _calibrateYEnabled; set => SetProperty(ref _calibrateYEnabled, value); }
        public bool StartCalibrationEnabled
        {
            get => _startCalibrationEnabled;
            set => SetProperty(ref _startCalibrationEnabled, value);
        }
        private string _xCoord;
        private string _yCoord;
        private bool _calibrateXEnabled;
        private bool _calibrateYEnabled;
        private bool _startCalibrationEnabled = true;

        // Actions
        public ICommand StartCalibrationCommand => new Command(StartCalibration);
        public ICommand CentralizeXCommand => new Command(_ => XShift += XAngleRes);
        public ICommand CentralizeYCommand => new Command(_ => YShift += YAngleRes);
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
        public ICommand ApplyCommand => new Command(ApplyCalibration);

        // Non-binded
        private static Settings Settings => Settings.Default;
        private readonly Thread DataThread;
        private readonly DispatcherTimer TextBoxUpdateTimer;

        private float XAngleRes => XAngle - XShift;
        private float YAngleRes => YAngle - YShift;

        private float XShift = Settings.ANGLE_X_SHIFT;
        private float YShift = Settings.ANGLE_Y_SHIFT;

        private float XAngle = Settings.X_MAX_ANGLE;
        private float YAngle = Settings.Y_MAX_ANGLE;

        private bool IsRunningThread = true;

        private readonly NavigationStore _navigationStore;

        public CalibrationViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;

            _xCoord = $"{XAngle:F2}";
            _yCoord = $"{YAngle:F2}";

            DataThread = new(ReceiveFromDisk);

            TextBoxUpdateTimer = new(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(Settings.CALIBRATION_TIMEOUT)
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
                using var con = Connection.GetConnection(IPAddress.Parse(Settings.IP), Settings.PORT);

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
                _navigationStore.NavigateBack();
            }
        } 

        private void StartCalibration(object? parameter)
        {
            StartCalibrationEnabled = false;

            IsRunningThread = true;
            CalibrateXEnabled = true;
            CalibrateYEnabled = true;

            DataThread.Start();
            TextBoxUpdateTimer.Start();
        }

        private void ApplyCalibration(object? parameter)
        {
            IsRunningThread = false;

            TextBoxUpdateTimer.Stop();

            if (DataThread.IsAlive)
            {
                DataThread.Join();
            }

            Settings.X_MAX_ANGLE = Math.Abs(Convert.ToSingle(XCoord));
            Settings.Y_MAX_ANGLE = Math.Abs(Convert.ToSingle(YCoord));

            Settings.ANGLE_X_SHIFT = XShift;
            Settings.ANGLE_Y_SHIFT = YShift;

            Settings.Save();
            _navigationStore.NavigateBack();
        }
    }
}