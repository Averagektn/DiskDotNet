using Disk.Calculations.Impl;
using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Visual.Impl;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using FilePath = System.IO.Path;
using Localization = Disk.Properties.Localization;
using Path = Disk.Visual.Impl.Path;
using PolarPointF = Disk.Data.Impl.PolarPoint<float>;
using Settings = Disk.Properties.Config.Config;

namespace Disk
{
    public partial class PaintWindow : Window
    {
        private void ShowStats()
        {
            using var userAngleReader = FileReader<float>.Open(UsrAngLog, Settings.LOG_SEPARATOR);

            var dataset = userAngleReader.Get2DPoints().ToList();

            if (dataset.Count != 0)
            {
                var mx = Calculator2D.MathExp(dataset);
                var dispersion = Calculator2D.Dispersion(dataset);
                var deviation = Calculator2D.StandartDeviation(dataset);

/*                MessageBox.Show(
                $"""
                 {Localization.Paint_Score}: {Score}
                 {Localization.Paint_MathExp}: {mx}
                 {Localization.Paint_Dispersion}: {dispersion}
                 {Localization.Paint_StandartDeviation}: {deviation}
                 """);*/

                MessageBox.Show(Localization.Paint_Over);
            }
            else
            {
                MessageBox.Show(Localization.Paint_Over);
            }
        }

        private void StopGame()
        {
            Target?.Remove(PaintArea.Children);
            User?.Remove(PaintArea.Children);

            Drawables.Remove(Target);
            Scalables.Remove(Target);

            Drawables.Remove(User);
            Scalables.Remove(User);

            IsGame = false;

            NetworkThread.Join();

            User?.ClearOnShot();

            MoveTimer.Stop();
            ShotTimer.Stop();

            UserLogAng?.Dispose();
            UserLogWnd?.Dispose();
            UserLogCen?.Dispose();
            UserMovementLog?.Dispose();
            MapReader?.Dispose();
        }

        private void OnClosing(object? sender, CancelEventArgs e) => StopGame();

        private void OnSizeChanged(object sender, RoutedEventArgs e)
        {
            AllowedArea.RadiusX = PaintPanelCenterX;
            AllowedArea.RadiusY = PaintPanelCenterY;
            AllowedArea.Center = new(PaintPanelCenterX, PaintPanelCenterY);

            foreach (var elem in Scalables)
            {
                elem?.Scale(PaintPanelSize);
            }
        }

        private string GetInTargetFileName(int id) =>
            $"{CurrPath}{FilePath.DirectorySeparatorChar}in_tar_{id}.log";

        private string GetMovToTargetFileName(int id) =>
            $"{CurrPath}{FilePath.DirectorySeparatorChar}mov_to_tar_{id}.log";

        private string GetReachedFileName(int id) =>
            $"{CurrPath}{FilePath.DirectorySeparatorChar}tar_{id}_reached.log";

        private void CbTargets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PaintArea.Children.Clear();

            var selectedIndex = CbTargets.SelectedIndex;
            var roseFileName = GetInTargetFileName(selectedIndex + 1);
            var pathFileName = GetMovToTargetFileName(selectedIndex + 1);

            if (selectedIndex != -1 && Converter is not null && Target is not null)
            {
                if (RbRose.IsChecked ?? false)
                {
                    if (File.Exists(roseFileName))
                    {
                        using var userReader = FileReader<float>.Open(roseFileName, Settings.LOG_SEPARATOR);

                        var angRadius = (Converter.ToAngleX_FromLog(Target.Radius) +
                            Converter.ToAngleY_FromLog(Target.Radius)) / 2;

                        var dataset =
                            userReader
                            .Get2DPoints()
                            .Select(p =>
                                new PolarPointF(p.X - TargetCenters[selectedIndex].X, p.Y - TargetCenters[selectedIndex].Y))
                            .Where(p => Math.Abs(p.X) > angRadius && Math.Abs(p.Y) > angRadius).ToList();

                        var userRose = new Graph(dataset, PaintPanelSize, Brushes.LightGreen, 8);
                        userRose.Draw(PaintArea);
                        Scalables.Add(userRose);
                    }
                }
                else if (RbPath.IsChecked ?? false)
                {
                    if (File.Exists(pathFileName))
                    {
                        using var userPathReader = FileReader<float>.Open(pathFileName, Settings.LOG_SEPARATOR);

                        var userPath = new Path
                            (
                                userPathReader.Get2DPoints(), PaintPanelSize, new SizeF(X_ANGLE_SIZE, Y_ANGLE_SIZE),
                                new SolidColorBrush
                                (
                                    Color.FromRgb(Settings.USER_COLOR.R, Settings.USER_COLOR.G, Settings.USER_COLOR.B)
                                )
                            );
                        userPath.Draw(PaintArea);
                        Scalables.Add(userPath);
                    }
                }
            }
        }

        private void RbRose_Checked(object sender, RoutedEventArgs e)
        {
            CbTargets.Items.Clear();

            for (int i = 1; i < TargetID; i++)
            {
                CbTargets.Items.Add($"{Localization.Paint_WindRoseForTarget} {i}");
            }
        }

        private void RbPath_Checked(object sender, RoutedEventArgs e)
        {
            CbTargets.Items.Clear();

            for (int i = 1; i < TargetID; i++)
            {
                CbTargets.Items.Add($"{Localization.Paint_PathToTarget} {i}");
            }
        }

        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            StopGame();
            ShowStats();

            BtnStop.IsEnabled = false;

            for (int i = 1; i < TargetID; i++)
            {
                CbTargets.Items.Add($"{Localization.Paint_WindRoseForTarget} {i}");
            }

            CbTargets.Visibility = Visibility.Visible;
            RbPath.Visibility = Visibility.Visible;
            RbRose.Visibility = Visibility.Visible;
        }
    }
}
