﻿using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Visual.Impl;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media;
using Localization = Disk.Properties.Localization;
using Settings = Disk.Properties.Config;

namespace Disk
{
    /// <summary>
    ///     Interaction logic for PaintWindow.xaml
    /// </summary>
    public partial class PaintWindow : Window
    {
        private void ShotTimerElapsed(object? sender, EventArgs e)
        {
            if (Target is not null && User is not null)
            {
                var shotScore = Target.ReceiveShot(User.Shot());

                if (shotScore != 0 && Stopwatch.IsRunning)
                {
                    Stopwatch.Stop();

                    if (StartPoint is not null)
                    {
                        using var log = Logger.GetLogger(TargetReachedLogName);

                        var touchPoint = Converter?.ToAngle_FromWnd(User.Center);
                        var distance = touchPoint?.GetDistance(StartPoint);
                        var time = Stopwatch.Elapsed.TotalSeconds;
                        var avgSpeed = distance / time;

                        var message =
                            $"""
                            {Localization.Paint_Time}: {time:F2}
                            {Localization.Paint_AngleDistance}: {distance:F2}
                            {Localization.Paint_AngleSpeed}: {avgSpeed:F2}
                            """;

                        TblTime.Text = message;
                        log.Log(message);

                        lock (LockObject)
                        {
                            UserMovementLog?.Dispose();
                            UserMovementLog = Logger.GetLogger(OnTargetLogName);
                        }
                    }
                }

                Score += shotScore;
            }

            Title = $"{Localization.Paint_Score}: {Score}";
            TblScore.Text = $"{Localization.Paint_Score}: {Score}";

            if (Target?.IsFull ?? false)
            {
                var newCenter = MapReader?.GetXY();
                Target.Reset();

                if (newCenter is null)
                {
                    OnStopClick(this, new());
                }
                else if (Converter is not null)
                {
                    var wndCenter = Converter.ToWnd_FromRelative(newCenter);
                    Target.Move(wndCenter);

                    TargetCenters.Add(new(Converter.ToAngleX_FromWnd(wndCenter.X), Converter.ToAngleY_FromWnd(wndCenter.Y)));

                    Stopwatch = Stopwatch.StartNew();
                    TblTime.Text = string.Empty;

                    if (User is not null)
                    {
                        StartPoint = Converter.ToAngle_FromWnd(User.Center);
                    }

                    lock (LockObject)
                    {
                        UserMovementLog?.Dispose();
                        UserMovementLog = Logger.GetLogger(MovingToTargetLogName);
                    }

                    TargetID++;
                }
            }
        }

        private void MoveTimerElapsed(object? sender, EventArgs e)
            => User?.Move(ShiftedWndPos ?? User.Center);

        private void NetworkReceive()
        {
            try
            {
                using var con = Connection.GetConnection(IPAddress.Parse(Settings.IP), Settings.PORT);

                while (IsGame)
                {
                    CurrentPos = con.GetXYZ();
                }
            }
            catch
            {
                MessageBox.Show(Localization.Paint_ConnectionLost);
                Application.Current.Dispatcher.BeginInvoke(new Action(() => Close()));
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Converter = new(SCREEN_INI_SIZE, new(X_ANGLE_SIZE, Y_ANGLE_SIZE));

            if (!Directory.Exists(CurrPath))
            {
                Directory.CreateDirectory(CurrPath);
            }

            if (MapFilePath != string.Empty)
            {
                MapReader = FileReader<float>.Open(MapFilePath);
                Target = new(Converter.ToWnd_FromRelative(MapReader.GetXY() ?? new(0.5f, 0.5f)),
                    Settings.TARGET_INI_RADIUS, SCREEN_INI_SIZE, TargetHP);
            }
            else
            {
                MessageBox.Show(Localization.Paint_EmptyMap);
            }

            UserLogWnd = Logger.GetLogger(UsrWndLog);
            UserLogAng = Logger.GetLogger(UsrAngLog);
            UserLogCen = Logger.GetLogger(UsrCenLog);
            UserMovementLog = Logger.GetLogger(UsrMovementLog);

            XAxis = new(new(0, SCREEN_INI_CENTER_X), new((int)SCREEN_INI_SIZE.Width, SCREEN_INI_CENTER_Y), SCREEN_INI_SIZE,
                Brushes.Black);
            YAxis = new(new(SCREEN_INI_CENTER_X, 0), new(SCREEN_INI_CENTER_X, (int)SCREEN_INI_SIZE.Height), SCREEN_INI_SIZE,
                Brushes.Black);
            PaintToDataBorder = new(new((int)SCREEN_INI_SIZE.Width, 0), new((int)SCREEN_INI_SIZE.Width,
                (int)SCREEN_INI_SIZE.Height), SCREEN_INI_SIZE, Brushes.Black);

            User = new(new(SCREEN_INI_CENTER_X, SCREEN_INI_CENTER_Y), Settings.USER_INI_RADIUS, Settings.USER_INI_SPEED,
                UserBrush, SCREEN_INI_SIZE);
            User.OnShot += UserLogWnd.LogLn;
            User.OnShot += (p) => UserLogAng.LogLn(Converter?.ToAngle_FromWnd(p));
            User.OnShot += (p) => UserLogCen.LogLn(Converter?.ToLogCoord(p));
            User.OnShot += (p) => UserMovementLog.LogLn(Converter?.ToAngle_FromWnd(p));

            Drawables.Add(XAxis); Drawables.Add(YAxis); Drawables.Add(PaintToDataBorder); Drawables.Add(Target);
            Drawables.Add(User);
            Scalables.Add(XAxis); Scalables.Add(YAxis); Scalables.Add(PaintToDataBorder); Scalables.Add(Target);
            Scalables.Add(User); Scalables.Add(Converter);

            foreach (var elem in Drawables)
            {
                elem?.Draw(PaintArea);
            }

            foreach (var elem in Scalables)
            {
                elem?.Scale(PaintPanelSize);
            }

            NetworkThread.Start();

            MoveTimer.Start();
            ShotTimer.Start();
        }
    }
}
