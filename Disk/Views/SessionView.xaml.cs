using Disk.Entities;
using Disk.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Disk.Views;

public partial class SessionView : UserControl
{
    private SessionViewModel? _viewModel => DataContext as SessionViewModel;

    public SessionView()
    {
        InitializeComponent();
    }

    private void SessionsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            if (e.EditingElement is TextBox cell)
            {
                var note = cell.Text;

                if (e.Row.Item is Attempt attempt && attempt.AttemptResult is not null)
                {
                    attempt.AttemptResult.Note = note;
                    _viewModel?.UpdateAttemptResult(attempt.AttemptResult);
                }
            }
        }
    }

    private void SessionsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var point = e.GetPosition(SessionsDataGrid);
        var hitTest = VisualTreeHelper.HitTest(SessionsDataGrid, point);
        if (hitTest is null)
        {
            return;
        }

        var cell = FindParent<DataGridCell>(hitTest.VisualHit);
        if (cell is null)
        {
            return;
        }

        int columnIndex = cell.Column.DisplayIndex;

        const int NoteColumnIndex = 4;
        if (columnIndex != NoteColumnIndex)
        {
            _viewModel?.ShowAttemptCommand.Execute(null);
        }
    }

    private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject parent = VisualTreeHelper.GetParent(child);

        while (parent is not null and not T)
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        return parent as T;
    }
}
