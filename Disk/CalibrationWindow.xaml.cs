using Disk.Calculations.Impl;
using Disk.Data.Impl;
using System.ComponentModel;
using System.Net;
using System.Timers;
using System.Windows;
using Settings = Disk.Config.Config;
using Timer = System.Timers.Timer;

namespace Disk
{
    /// <summary>
    ///     Interaction logic for CalibrationWindow.xaml
    /// </summary>
    public partial class CalibrationWindow : Window
    {
        private static Settings Settings => Settings.Default;

        private readonly Thread DataThread;

        private readonly Timer TextBoxUpdateTimer;

        private float XAngleRes => XAngle - XShift;
        private float YAngleRes => YAngle - YShift;

        private float XShift = Settings.ANGLE_X_SHIFT;
        private float YShift = Settings.ANGLE_Y_SHIFT;

        private float XAngle = Settings.X_MAX_ANGLE;
        private float YAngle = Settings.Y_MAX_ANGLE;

        private bool IsRunningThread = true;

        /// <summary>
        /// 
        /// </summary>
        public CalibrationWindow()
        {
            InitializeComponent();

            Closing += OnClosing;

            TbXCoord.Text = XAngle.ToString();
            TbYCoord.Text = YAngle.ToString();

            DataThread = new(NetworkThreadProc);

            TextBoxUpdateTimer = new(Settings.MOVE_TIME);
            TextBoxUpdateTimer.Elapsed += OnTextBoxUpdateTimerElapsed;
        }

        private void OnTextBoxUpdateTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (BtnCalibrateX.IsEnabled)
                {
                    TbXCoord.Text = XAngleRes.ToString();
                }
                if (BtnCalibrateY.IsEnabled)
                {
                    TbYCoord.Text = YAngleRes.ToString();
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCentralizeXClick(object sender, RoutedEventArgs e) => XShift += XAngleRes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCentralizeYClick(object sender, RoutedEventArgs e) => YShift += YAngleRes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartCalibrationClick(object sender, RoutedEventArgs e)
        {
            BtnStartCalibration.IsEnabled = false;

            IsRunningThread = true;
            BtnCalibrateX.IsEnabled = true;
            BtnCalibrateY.IsEnabled = true;

            DataThread.Start();
            TextBoxUpdateTimer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
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
                        XAngle = Converter.ToAngle_FromRadian(data.X);
                        YAngle = Converter.ToAngle_FromRadian(data.Y);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Connection lost");
                Application.Current.Dispatcher.BeginInvoke(new Action(() => Close()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCalibrateXClick(object sender, RoutedEventArgs e)
        {
            BtnCalibrateX.IsEnabled = false;

            IsRunningThread = BtnCalibrateY.IsEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCalibrateYClick(object sender, RoutedEventArgs e)
        {
            BtnCalibrateY.IsEnabled = false;

            IsRunningThread = BtnCalibrateX.IsEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplyClick(object sender, RoutedEventArgs e) => Close();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosing(object? sender, CancelEventArgs e)
        {
            IsRunningThread = false;

            TextBoxUpdateTimer.Stop();

            if (DataThread.IsAlive)
            {
                DataThread.Join();
            }

            Settings.X_MAX_ANGLE = Math.Abs(Convert.ToSingle(TbXCoord.Text));
            Settings.Y_MAX_ANGLE = Math.Abs(Convert.ToSingle(TbYCoord.Text));

            Settings.ANGLE_X_SHIFT = XShift;
            Settings.ANGLE_Y_SHIFT = YShift;

            Settings.Save();
        }
    }
}
