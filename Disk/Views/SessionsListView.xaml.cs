using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Disk.Entities;
using Disk.ViewModels;

namespace Disk.Views;

public partial class SessionsListView : UserControl
{
    private SessionsListViewModel? ViewModel => DataContext as SessionsListViewModel;

    private readonly DispatcherTimer _leaveTimer;
    private bool _isMouseOverRow;
    private bool _isMouseOverMapPreview;

    public SessionsListView()
    {
        InitializeComponent();

        _leaveTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _leaveTimer.Tick += OnLeaveTimerTick;
    }

    private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        _isMouseOverRow = true;
        _leaveTimer.Stop();

        if (sender is DataGridRow row && row.DataContext is Session session && ViewModel.HoveredSession != session)
        {
            ViewModel.HoveredSession = session;
            if (SavedRow is not null)
            {
                SavedRow.Background = Brushes.White;
            }
            SavedRow = row;
            row.Background = Brushes.LightCyan;

            _ = Dispatcher.BeginInvoke(() =>
            {
                if (row is null || !row.IsVisible)
                {
                    return;
                }

                Point rowScreenPos = row.PointToScreen(new Point(0, 0));
                double screenHeight = SystemParameters.PrimaryScreenHeight;
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double popupHeight = MapPopup.Height;
                double popupWidth = MapPopup.Width;

                MapPopup.HorizontalOffset = screenWidth - rowScreenPos.X - row.ActualWidth > popupWidth ? rowScreenPos.X + row.ActualWidth : screenWidth - popupWidth;

                MapPopup.VerticalOffset = screenHeight - rowScreenPos.Y - (row.ActualHeight * 2) > popupHeight ? rowScreenPos.Y : rowScreenPos.Y - popupHeight;
            }, DispatcherPriority.Loaded);
        }
    }

    private DataGridRow? SavedRow;
    private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
    {
        if (_isMouseOverMapPreview)
        {
            return;
        }

        if (sender is DataGridRow row && !row.IsMouseOver)
        {
            _isMouseOverRow = false;
            _leaveTimer.Stop();
            _leaveTimer.Start();
        }
    }

    private void OnLeaveTimerTick(object? sender, EventArgs e)
    {
        _leaveTimer.Stop();

        if (!_isMouseOverRow && ViewModel is not null)
        {
            ViewModel.HoveredSession = null;
            if (SavedRow is not null)
            {
                SavedRow.Background = Brushes.White;
            }
        }
    }

    private void MapPreviewView_MouseEnter(object sender, MouseEventArgs e)
    {
        _isMouseOverMapPreview = true;
        _leaveTimer.Stop();
    }

    private void MapPreviewView_MouseLeave(object sender, MouseEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        _isMouseOverMapPreview = false;
        _leaveTimer.Stop();
        _leaveTimer.Start();
    }
}
