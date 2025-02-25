using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Navigators;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Disk.Visual.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel;

public class MapCreatorViewModel(ModalNavigationStore modalNavigationStore) : ObserverViewModel
{
    private static int IniWidth => Settings.Default.IniScreenWidth;
    private static int IniHeight => Settings.Default.IniScreenHeight;
    private readonly List<NumberedTarget> _targets = [];
    public Target? MovingTarget;
    public virtual ICommand CancelCommand => new Command(_ => IniNavigationStore.Close());

    public Target? GetTarget(Point mousePos)
    {
        var x = (int)mousePos.X;
        var y = (int)mousePos.Y;

        return _targets.FindLast(target => target.Contains(new Point2D<int>(x, y)));
    }

    // rework saving
    //public ICommand Save => new Command(_ => SaveMap());
    public void SaveMap(double actualWidth, double actualHeight)
    {
        var converter = new Converter((int)actualWidth, (int)actualHeight, Settings.Default.XMaxAngle, Settings.Default.YMaxAngle);
        if (_targets.Count != 0)
        {
            var map = _targets
                .Select(t => new Point2D<float>(converter.ToAngleX_FromWnd(t.Center.X), converter.ToAngleX_FromWnd(t.Center.Y)))
                .ToList();

            MapNamePickerNavigator.Navigate(modalNavigationStore, map);
        }

        IniNavigationStore.Close();
    }

    public void RemoveTarget(Point mousePos)
    {
        var x = (int)mousePos.X;
        var y = (int)mousePos.Y;

        var removableTagets = _targets.Where(target => target.Contains(new(x, y)));
        foreach (var target in removableTagets)
        {
            target.Remove();
        }
        _ = _targets.RemoveAll(target => target.Contains(new(x, y)));

        for (int i = 0; i < _targets.Count; i++)
        {
            _targets[i].UpdateNumber(i + 1);
        }
    }

    public void ScaleTargets(Size size)
    {
        foreach (var target in _targets)
        {
            target?.Scale();
        }
    }

    public void AddTarget(Point mousePos, Size renderSize, Canvas canvas)
    {
        var newTarget = GetIniCoordTarget(mousePos.X, mousePos.Y, canvas);
        newTarget.Scale();
        newTarget.Draw();
        _targets.Add(newTarget);
    }

    private NumberedTarget GetIniCoordTarget(double actualX, double actualY, Canvas canvas)
    {
        return new(
            center: new Point2D<int>
            (
                (int)(actualX / canvas.RenderSize.Width * IniWidth),
                (int)(actualY / canvas.RenderSize.Height * IniHeight)
            ),
            radius: Settings.Default.IniTargetRadius,
            parent: canvas,
            number: _targets.Count + 1,
            iniSize: new(IniWidth, IniHeight),
            converter: new Converter(800, 800, 40, 40)
        );
    }
}