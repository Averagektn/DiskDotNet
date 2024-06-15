using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Visual.Impl;
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

namespace Disk.View.PaintWindow
{
    public partial class PaintView : UserControl
    {
        private void StopGame()
        {
            Target.Remove(PaintArea.Children);
            User.Remove(PaintArea.Children);

            Drawables.Remove(Target);
            Scalables.Remove(Target);

            Drawables.Remove(User);
            Scalables.Remove(User);

            User.ClearOnShot();

            MoveTimer.Stop();
            ShotTimer.Stop();
        }

        private void OnClosing(object? sender, RoutedEventArgs e)
        {
            StopGame();
        }

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
            $"{ViewModel.CurrPath}{FilePath.DirectorySeparatorChar}in_tar_{id}.log";

        private string GetMovToTargetFileName(int id) =>
            $"{ViewModel.CurrPath}{FilePath.DirectorySeparatorChar}mov_to_tar_{id}.log";

        private void CbTargets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PaintArea.Children.Clear();

            var selectedIndex = CbTargets.SelectedIndex;
            var roseFileName = GetInTargetFileName(selectedIndex + 1);
            var pathFileName = GetMovToTargetFileName(selectedIndex + 1);

            if (selectedIndex != -1)
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
                                new PolarPointF(p.X - ViewModel.TargetCenters[selectedIndex].X, p.Y - ViewModel.TargetCenters[selectedIndex].Y))
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
                                userPathReader.Get2DPoints(), PaintPanelSize, new SizeF(20.0f/*XAngleSize*/, /*YAngleSize*/20.0f),
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

            for (int i = 1; i < ViewModel.TargetId + 1; i++)
            {
                CbTargets.Items.Add($"{Localization.Paint_WindRoseForTarget} {i}");
            }
        }

        private void RbPath_Checked(object sender, RoutedEventArgs e)
        {
            CbTargets.Items.Clear();

            for (int i = 1; i < ViewModel.TargetId + 1; i++)
            {
                CbTargets.Items.Add($"{Localization.Paint_PathToTarget} {i}");
            }
        }

        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            StopGame();
            ViewModel.SaveSessionResult();

            BtnStop.IsEnabled = false;

            for (int i = 1; i < ViewModel.TargetId + 1; i++)
            {
                CbTargets.Items.Add($"{Localization.Paint_WindRoseForTarget} {i}");
            }

            CbTargets.Visibility = Visibility.Visible;
            RbPath.Visibility = Visibility.Visible;
            RbRose.Visibility = Visibility.Visible;
        }
    }
}
