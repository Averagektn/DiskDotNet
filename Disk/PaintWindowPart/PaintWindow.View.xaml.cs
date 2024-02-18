using Disk.Calculations.Impl;
using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Visual.Impl;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FilePath = System.IO.Path;
using Path = Disk.Visual.Impl.Path;
using PolarPointF = Disk.Data.Impl.PolarPoint<float>;
using Settings = Disk.Config.Config;

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

                MessageBox.Show(
                $"""
                Счет: {Score}
                Среднее смещение от центра: {mx}
                Дисперсия: {dispersion}
                Среднее отклонение от центра: {deviation}
                """);
            }
            else
            {
                MessageBox.Show("Конец");
            }
        }

        private void StopGame()
        {
            if (Target is not null)
            {
                Target.Remove(PaintArea.Children);
                User?.Remove(PaintArea.Children);

                Drawables.Remove(Target);
                Scalables.Remove(Target);

                Drawables.Remove(User);
                Scalables.Remove(User);
            }

            IsGame = false;

            NetworkThread.Join();

            MoveTimer.Stop();
            ShotTimer.Stop();

            UserLogAng?.Dispose();
            UserLogWnd?.Dispose();
            UserLogCen?.Dispose();
            UserMovementLog?.Dispose();
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

        private void CbTargets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PaintArea.Children.Clear();
            foreach (var elem in Drawables)
            {
                elem?.Draw(PaintArea);
            }

            var selectedIndex = CbTargets.SelectedIndex;
            var roseFileName = $"{CurrPath}{FilePath.DirectorySeparatorChar}В мишени {selectedIndex + 1}.log";
            var pathFileName = $"{CurrPath}{FilePath.DirectorySeparatorChar}Движение к мишени {selectedIndex + 1}.log";

            if (selectedIndex != -1 && Converter is not null && Target is not null)
            {
                if (RbRose.IsChecked ?? false)
                {
                    if (File.Exists(roseFileName))
                    {
                        using var userReader = FileReader<float>.Open(roseFileName, Settings.LOG_SEPARATOR);

                        var angRadius = Converter.ToAngleX_FromLog(Target.Radius) +
                            Converter.ToAngleY_FromLog(Target.Radius) / 2;

                        var a = userReader.Get2DPoints().ToList();
                        var dataset =
                            a
                            .Select(p => new PolarPointF(p.X - TargetCenters[selectedIndex].X, p.Y -
                            TargetCenters[selectedIndex].Y, null))
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

                        var userPath = new Path(userPathReader.Get2DPoints(), PaintPanelSize, new(X_ANGLE_SIZE, Y_ANGLE_SIZE),
                            new SolidColorBrush(Color.FromRgb(Settings.USER_COLOR.R, Settings.USER_COLOR.G,
                            Settings.USER_COLOR.B)));

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
                CbTargets.Items.Add($"Роза ветров для цели {i}");
            }
        }

        private void RbPath_Checked(object sender, RoutedEventArgs e)
        {
            CbTargets.Items.Clear();

            for (int i = 1; i < TargetID; i++)
            {
                CbTargets.Items.Add($"Путь к цели {i}");
            }
        }

        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            StopGame();
            ShowStats();

            BtnStop.IsEnabled = false;

            for (int i = 1; i < TargetID; i++)
            {
                CbTargets.Items.Add($"Роза ветров для цели {i}");
            }
            CbTargets.Visibility = Visibility.Visible;
            RbPath.Visibility = Visibility.Visible;
            RbRose.Visibility = Visibility.Visible;
        }
    }
}
