using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
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
                        lock (LockObject)
                        {
                            UserMovementLog?.Dispose();
                            UserMovementLog = Logger.GetLogger(OnTargetLogName);
                        }

                        double distance = 0;
                        using (var reader = FileReader<float>.Open(MovingToTargetLogName))
                        {
                            var currPoint = reader.GetXY() ?? StartPoint;
                            var nextPoint = reader.GetXY();

                            while (nextPoint is not null)
                            {
                                distance += currPoint.GetDistance(nextPoint);
                                currPoint = nextPoint;
                                nextPoint = reader.GetXY();
                            }
                        }

                        var touchPoint = Converter?.ToAngle_FromWnd(User.Center);
                        var time = Stopwatch.Elapsed.TotalSeconds;
                        var avgSpeed = distance / time;
                        var approachSpeed = StartPoint.GetDistance(touchPoint!) / time;

                        var message =
                            $"""
                            {Localization.Paint_Time}: {time:F2}
                            {Localization.Paint_AngleDistance}: {distance:F2}
                            {Localization.Paint_AngleSpeed}: {avgSpeed:F2}
                            {Localization.Paint_ApproachSpeed}: {approachSpeed:F2}
                            """;

                        TblTime.Text = message;
                        using (var log = Logger.GetLogger(TargetReachedLogName))
                        {
                            log.Log(message);
                        }

                        TargetID++;
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

                    TargetCenters.Add(Converter.ToAngle_FromWnd(wndCenter));

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
                }
            }
        }

        private void MoveTimerElapsed(object? sender, EventArgs e)
        {
            if (ShiftedWndPos is not null && AllowedArea.FillContains(ShiftedWndPos.ToPoint()))
            {
                User?.Move(ShiftedWndPos);
            }
        }

        private void NetworkReceive()
        {
            try
            {
                using var con = Connection.GetConnection(IPAddress.Parse(Settings.IP), Settings.PORT);

                while (IsGame)
                {
                    CurrentPos = con.GetXYZ();
                    //???
                    //Thread.Sleep(Settings.MOVE_TIME / 2);
                }
            }
            catch
            {
                MessageBox.Show(Localization.Paint_ConnectionLost);
                Application.Current.Dispatcher.BeginInvoke(new Action(Close));
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
                var center = MapReader.GetXY() ?? new(0.5f, 0.5f);
                Target = new(Converter.ToWnd_FromRelative(center),
                    Settings.TARGET_INI_RADIUS + 5, SCREEN_INI_SIZE, TargetHP);
                TargetCenters.Add(center);
            }
            else
            {
                MessageBox.Show(Localization.Paint_EmptyMap);
            }

            UserLogWnd = Logger.GetLogger(UsrWndLog);
            UserLogAng = Logger.GetLogger(UsrAngLog);
            UserLogCen = Logger.GetLogger(UsrCenLog);
            UserMovementLog = Logger.GetLogger(MovingToTargetLogName);

            StartPoint = new(0.0f, 0.0f);

            /*            User = new("/Properties/WinniePooh.png", new(SCREEN_INI_CENTER_X, SCREEN_INI_CENTER_Y), Settings.USER_INI_SPEED,
                            new(100, 100), SCREEN_INI_SIZE);*/
            User.OnShot += UserLogWnd.LogLn;
            User.OnShot += (p) => UserLogAng.LogLn(Converter?.ToAngle_FromWnd(p));
            User.OnShot += (p) => UserLogCen.LogLn(Converter?.ToLogCoord(p));
            User.OnShot += (p) => UserMovementLog.LogLn(Converter?.ToAngle_FromWnd(p));

            Drawables.Add(Target); Drawables.Add(User);
            Scalables.Add(Target); Scalables.Add(User); Scalables.Add(Converter);

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
