using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Entities;
using Disk.Service.Implementation;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Localization = Disk.Properties.Langs.SessionResult.SessionResultLocalization;

namespace Disk.ViewModel
{
    public class SessionResultViewModel(NavigationStore navigationStore) : ObserverViewModel
    {
        private Session _currentSession = null!;
        public Session CurrentSession
        {
            get => _currentSession;
            set
            {
                _currentSession = value;
                TargetCenters = JsonConvert.DeserializeObject<List<Point2D<float>>>(CurrentSession.MapNavigation.CoordinatesJson)!;
                PathsToTargets = CurrentSession.PathToTargets
                    .Select(ptt => JsonConvert.DeserializeObject<List<Point2D<float>>>(ptt.CoordinatesJson)!)
                    .ToList() ?? [];
                PathsInTargets = CurrentSession.PathInTargets
                    .Select(pit => JsonConvert.DeserializeObject<List<Point2D<float>>>(pit.CoordinatesJson)!)
                    .ToList() ?? [];

                IsPathChecked = true;
                SelectedIndex = 0;

                NewItemSelectedCommand.Execute(null);
            }
        }

        public ObservableCollection<string> Indices { get; set; } = [];
        public Converter Converter { get; set; } = DrawableFabric.GetIniConverter();
        public IEnumerable<(bool IsNewTarget, Point2D<float> Point)> FullPath
        {
            get
            {
                Point2D<float> lastPoint = null!;

                for (int i = 0; i < TargetCenters.Count; i++)
                {
                    var ptt = JsonConvert.DeserializeObject<List<Point2D<float>>>(CurrentSession.PathToTargets.ElementAt(i).CoordinatesJson)!;
                    foreach (var point in ptt)
                    {
                        yield return (false, point);
                    }

                    var pit = JsonConvert.DeserializeObject<List<Point2D<float>>>(CurrentSession.PathInTargets.ElementAt(i).CoordinatesJson)!;
                    foreach (var point in pit)
                    {
                        lastPoint = point;
                        yield return (false, point);
                    }

                    yield return (true, lastPoint);
                }
            }
        }

        private List<Point2D<float>> _targetCenters = [];
        public List<Point2D<float>> TargetCenters
        {
            get => _targetCenters;
            set
            {
                _targetCenters = value;
                FillTargetsComboBox();
            }
        }
        private List<List<Point2D<float>>> PathsToTargets { get; set; } = [];
        private List<List<Point2D<float>>> PathsInTargets { get; set; } = [];

        private string _message = string.Empty;
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        private bool _isDiagramChecked = false;
        public bool IsDiagramChecked { get => _isDiagramChecked; set => SetProperty(ref _isDiagramChecked, value); }

        private bool _isPathChecked = false;
        public bool IsPathChecked { get => _isPathChecked; set => SetProperty(ref _isPathChecked, value); }

        private int _selectedIndex = -1;
        public int SelectedIndex { get => _selectedIndex; set => SetProperty(ref _selectedIndex, value); }

        public bool ShowPathInTarget { get; set; }
        public bool ShowPathToTarget { get; set; }

        public ICommand NavigateBackCommand => new Command(_ => navigationStore.Close());

        public ICommand NewItemSelectedCommand => new Command(_ => Message =
                $"""
                {Localization.StandartDeviation}: {CurrentSession.SessionResult?.Deviation:F2}
                {Localization.Dispersion}: {CurrentSession.SessionResult?.Dispersion:F2}
                {Localization.MathExp}: {CurrentSession.SessionResult?.MathExp:F2}
                {Localization.AngleDistance}: {CurrentSession.PathToTargets.ElementAt(SelectedIndex).AngleDistance:F2}
                {Localization.AngleSpeed}: {CurrentSession.PathToTargets.ElementAt(SelectedIndex).AngleSpeed:F2}
                {Localization.ApproachSpeed}: {CurrentSession.PathToTargets.ElementAt(SelectedIndex).ApproachSpeed:F2}
                {Localization.Time}: {CurrentSession.PathToTargets.ElementAt(SelectedIndex).Time:F2}
                {Localization.Precision}: {CurrentSession.PathInTargets.ElementAt(SelectedIndex).Precision:F2}
                """);

        public void FillTargetsComboBox()
        {
            for (int i = 0; i < TargetCenters.Count; i++)
            {
                Indices.Add($"{Localization.Target} {i + 1}");
            }
        }

        public List<IStaticFigure> GetPathAndRose(Size paintAreaSize)
        {
            if (SelectedIndex == -1)
            {
                return [];
            }

            if (IsDiagramChecked)
            {
                return [GetGraph(paintAreaSize)];
            }
            else if (IsPathChecked)
            {
                if (PathsToTargets.Count <= SelectedIndex || PathsToTargets[SelectedIndex].Count == 0)
                {
                    _ = MessageBox.Show(Localization.NoContentForPathError);
                    return [];
                }
                var converter = DrawableFabric.GetIniConverter();
                var targetCenter = converter.ToWnd_FromRelative(TargetCenters[SelectedIndex]);
                var targetToDraw = DrawableFabric.GetIniProgressTarget(targetCenter);

                var userToDraw = DrawableFabric.GetIniUser(string.Empty);
                userToDraw.Move(converter.ToWndCoord(PathsToTargets[SelectedIndex][0]));
                var res = new List<IStaticFigure> { userToDraw, targetToDraw };

                var pathToTarget = new Path(PathsToTargets[SelectedIndex], Converter, new SolidColorBrush(Colors.Green));
                var pathInTarget = new Path(PathsInTargets[SelectedIndex], Converter, new SolidColorBrush(Colors.Blue));
                if (ShowPathToTarget)
                {
                    res.Add(pathToTarget);
                }
                if (ShowPathInTarget)
                {
                    res.Add(pathInTarget);
                }

                return res;
            }

            return [];
        }

        private Graph GetGraph(Size paintAreaSize)
        {
            if (PathsInTargets.Count <= SelectedIndex || PathsInTargets[SelectedIndex].Count == 0)
            {
                _ = MessageBox.Show(Localization.NoContentForDiagramError);
                return new Graph([], paintAreaSize, Brushes.LightGreen, 8);
            }

            var target = DrawableFabric.GetIniProgressTarget(new(0, 0));
            target.Scale(paintAreaSize);

            var angRadius = (Converter.ToAngleX_FromWnd(target.Radius) + Converter.ToAngleY_FromWnd(target.Radius)) / 2;

            var angCenter = Converter.ToAngle_FromWnd(Converter.ToWnd_FromRelative(TargetCenters[SelectedIndex]));
            var dataset =
                PathsInTargets[SelectedIndex]
                .Select(p => new PolarPoint<float>(p.X - angCenter.X, p.Y - angCenter.Y))
                .Where(p => Math.Abs(p.X) > angRadius && Math.Abs(p.Y) > angRadius)
                .ToList();

            return new Graph(dataset, paintAreaSize, Brushes.LightGreen, 8);
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
            base.Dispose();

            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }
    }
}