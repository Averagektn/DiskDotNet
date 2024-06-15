using Disk.Entities;
using Disk.Sessions;
using Disk.Visual.Impl;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Localization = Disk.Properties.Localization;

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
            var angleShot = Converter.ToAngle_FromWnd(shot);

            // pit
            bool isPathInTargetStarts = shotScore != 0 && PathToTargetStopwatch.IsRunning;
            if (isPathInTargetStarts)
            {
                // final point in path to target
                PathToTargetCoords.Add(angleShot);
                PathToTargetStopwatch.Stop();

                var ptt = ViewModel.SwitchToPathInTarget(shot);
                var message =
                    $"""
                        {Localization.Paint_Time}: {ptt.Time:F2}
                        {Localization.Paint_AngleDistance}: {ptt.AngleDistance:F2}
                        {Localization.Paint_AngleSpeed}: {ptt.AngleSpeed:F2}
                        {Localization.Paint_ApproachSpeed}: {ptt.ApproachSpeed:F2}
                     """;

                TblTime.Text = message;
            }

            bool isPathInTarget = !PathToTargetStopwatch.IsRunning;
            if (isPathInTarget)
            {
                PathInTargetCoords.Add(angleShot);
                ViewModel.PathsInTargets[ViewModel.TargetId - 1].Add(angleShot);
            }
            else
            {
                ViewModel.PathsToTargets[ViewModel.TargetId - 1].Add(angleShot);
            }

            TblScore.Text = $"{Localization.Paint_Score}: {ViewModel.Score}";

            // ptt
            bool isPathToTargetStarts = Target.IsFull;
            if (isPathToTargetStarts)
            {
                ViewModel.PathsToTargets.Add([]);
                var pit = new PathInTarget()
                {
                    CoordinatesJson = JsonConvert.SerializeObject(PathInTargetCoords),
                    Session = AppointmentSession.CurrentSession.Id,
                    TargetId = ViewModel.TargetId,
                };
                ViewModel.SavePathInTarget(pit);

                PathInTargetCoords = [];

                // TargetId++
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

                    PathToTargetStopwatch = Stopwatch.StartNew();
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
