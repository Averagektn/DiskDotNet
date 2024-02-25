using Disk.Data.Impl;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class CalibrationViewModel : INotifyPropertyChanged
    {
        // Properties
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
        public ICommand StartCalibration => new Command(OnStartCalibrationClick);
        public ICommand CentralizeX => new Command(OnCentralizeXClick);
        public ICommand CentralizeY => new Command(OnCentralizeYClick);
        public ICommand CalibrateX => new Command(OnCalibrateXClick);
        public ICommand CalibrateY => new Command(OnCalibrateYClick);
        public ICommand Apply => new Command(OnApplyClick);

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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = (newValue);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

        public CalibrationViewModel()
        {
            _xCoord = $"{XAngle:F2}";
            _yCoord = $"{YAngle:F2}";

            DataThread = new(NetworkThreadProc);

            TextBoxUpdateTimer = new(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(Settings.CALIBRATION_TIMEOUT)
            };
            TextBoxUpdateTimer.Tick += OnTextBoxUpdateTimerElapsed;
        }

        private void OnTextBoxUpdateTimerElapsed(object? sender, EventArgs e)
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

        private void NetworkThreadProc()
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
                MessageBox.Show(Properties.Localization.Calibration_ConnectionLost);
                Application.Current.Windows.OfType<CalibrationWindow>().First().Close();
            }
        }

        private void OnCentralizeXClick(object? parameter) => XShift += XAngleRes;

        private void OnCentralizeYClick(object? parameter) => YShift += YAngleRes;

        private void OnStartCalibrationClick(object? parameter)
        {
            StartCalibrationEnabled = false;

            IsRunningThread = true;
            CalibrateXEnabled = true;
            CalibrateYEnabled = true;

            DataThread.Start();
            TextBoxUpdateTimer.Start();
        }

        private void OnCalibrateXClick(object? parameter)
        {
            CalibrateXEnabled = false;

            IsRunningThread = CalibrateYEnabled;
        }

        private void OnCalibrateYClick(object? parameter)
        {
            CalibrateYEnabled = false;

            IsRunningThread = CalibrateXEnabled;
        }

        private void OnApplyClick(object? parameter)
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
            Application.Current.Windows.OfType<CalibrationWindow>().First().Close();
        }
    }
}