﻿using Disk.Data.Impl;
using Disk.Entities;
using Disk.Sessions;
using Disk.Visual.Impl;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Localization = Disk.Properties.Localization;
using Settings = Disk.Properties.Config;

namespace Disk.View.PaintWindow
{
    /// <summary>
    /// Interaction logic for PaintWindowView.xaml
    /// </summary>
    public partial class PaintView : UserControl
    {
        private void ShotTimerElapsed(object? sender, EventArgs e)
        {
            var shot = User.Shot();
            var shotScore = Target.ReceiveShot(shot);
            var a = Converter.ToAngle_FromWnd(shot);

            if (shotScore != 0 && Stopwatch.IsRunning)
            {
                PathToTargetCoords.Add(a);
                Stopwatch.Stop();

                if (PathStartingPoint is not null)
                {
                    /*                    lock (LockObject)
                                        {
                                            UserMovementLog.Dispose();
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
                                        }*/

                    var touchPoint = Converter?.ToAngle_FromWnd(User.Center);
                    var time = Stopwatch.Elapsed.TotalSeconds;
                    var avgSpeed = 0;//distance / time;
                    var approachSpeed = PathStartingPoint.GetDistance(touchPoint!) / time;

                    var ptt = new PathToTarget()
                    {
                        AngleDistance = 0,//distance,
                        AngleSpeed = avgSpeed,
                        ApproachSpeed = approachSpeed,
                        CoordinatesJson = JsonConvert.SerializeObject(PathToTargetCoords),
                        TargetNum = TargetID,
                        Session = AppointmentSession.CurrentSession.Id,
                        Time = time
                    };
                    ViewModel.SavePathToTarget(ptt);
                    PathToTargetCoords = [];

                    var message =
                        $"""
                            {Localization.Paint_Time}: {time:F2}
                            {Localization.Paint_AngleDistance}: distance:F2
                            {Localization.Paint_AngleSpeed}: {avgSpeed:F2}
                            {Localization.Paint_ApproachSpeed}: {approachSpeed:F2}
                            """
                    ;

                    TblTime.Text = message;
                    /*                    using (var log = Logger.GetLogger(TargetReachedLogName))
                                        {
                                            log.Log(message);
                                        }
                    */
                    TargetID++;
                }
            }
            else
            {
                PathInTargetCoords.Add(a);
            }

            TblScore.Text = $"{Localization.Paint_Score}: {ViewModel.Score}";

            if (Target.IsFull)
            {
                var pit = new PathInTarget()
                {
                    CoordinatesJson = JsonConvert.SerializeObject(PathInTargetCoords),
                    Session = AppointmentSession.CurrentSession.Id,
                    TargetId = TargetID,
                };
                ViewModel.SavePathInTarget(pit);

                PathInTargetCoords = [];
                var newCenter = ViewModel.NextTargetCenter;
                Target.Reset();

                if (newCenter is null)
                {
                    OnStopClick(this, new());
                }
                else
                {
                    var wndCenter = Converter.ToWnd_FromRelative(newCenter);
                    Target.Move(wndCenter);

                    //TargetCenters.Add(Converter.ToAngle_FromWnd(wndCenter));

                    Stopwatch = Stopwatch.StartNew();
                    TblTime.Text = string.Empty;

                    if (User is not null)
                    {
                        PathStartingPoint = Converter.ToAngle_FromWnd(User.Center);
                    }
                }
            }
        }

        private void MoveTimerElapsed(object? sender, EventArgs e)
        {
            if (ShiftedWndPos is not null && AllowedArea.FillContains(ShiftedWndPos.ToPoint()))
            {
                User.Move(ShiftedWndPos);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            User = ViewModel.GetUser();
            Target = ViewModel.GetProgressTarget();

            //TargetCenters.Add(new Point2D<float>((float)Target.Center.XDbl, (float)Target.Center.YDbl));

            PathStartingPoint = Converter.ToAngle_FromWnd(User.Center);

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

            MoveTimer.Start();
            ShotTimer.Start();
        }
    }
}
