using Disk.Db.Context;
using Disk.Entities;
using Disk.Navigators;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Disk.ViewModel;

public class SessionsListViewModel(DiskContext database, NavigationStore navigationStore) : ObserverViewModel
{
    private const int SessionsPerPage = 15;
    private int _currPage;

    private bool _isMapVisible;
    public bool IsMapVisible { get => HoveredSession is not null; set => SetProperty(ref _isMapVisible, value); }

    private Session? _hoveredSession = null;
    public Session? HoveredSession
    {
        get => _hoveredSession;
        set
        {
            _ = SetProperty(ref _hoveredSession, value);
            OnPropertyChanged(nameof(IsMapVisible));
        }
    }

    private Session? _selectedSession = null;
    public Session? SelectedSession
    {
        get => _selectedSession;
        set
        {
            _ = SetProperty(ref _selectedSession, value);
        }
    }

    private Patient _patient = null!;
    public required Patient Patient
    {
        get => _patient;
        set
        {
            _patient = value;

            _ = Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await UpdateSessionsAsync();
                IsNextEnabled = _currPage < TotalPages - 1;
            }).Task.ContinueWith(e =>
            {
                if (e.Exception is not null)
                {
                    Log.Error($"{e.Exception.Message} \n {e.Exception.StackTrace}");
                }
            });
        }
    }

    private ObservableCollection<Session> _sessions = [];
    public ObservableCollection<Session> Sessions
    {
        get => _sessions;
        set => SetProperty(ref _sessions, value);
    }

    private DateTime? _selectedDate;
    public DateTime? SelectedDate
    {
        get => _selectedDate;
        set
        {
            _ = SetProperty(ref _selectedDate, value);
            _ = Application.Current.Dispatcher.InvokeAsync(UpdateSessionsAsync).Task.ContinueWith(e =>
            {
                if (e.Exception is not null)
                {
                    Log.Error($"{e.Exception.Message} \n {e.Exception.StackTrace}");
                }
            });
        }
    }

    private bool _isNextEnabled;
    public bool IsNextEnabled { get => _isNextEnabled; set => SetProperty(ref _isNextEnabled, value); }

    private bool _isPreviousEnabled;
    public bool IsPreviousEnabled { get => _isPreviousEnabled; set => SetProperty(ref _isPreviousEnabled, value); }

    private int TotalPages
    {
        get
        {
            var sessionsCount = database.Sessions.Count();

            return (int)Math.Ceiling((double)sessionsCount / SessionsPerPage);
        }
    }

    public ICommand TableRowEnterCommand => new Command(elem =>
    {
        if (elem is Session s)
        {
            _ = MessageBox.Show(s.MapNavigation.Name);
        }
    });

    public ICommand ConfigureSessionCommand => new Command(_ =>
        ConfigureSessionNavigator.NavigateWithBar(this, navigationStore, Patient));

    public ICommand CancelDateCommand => new AsyncCommand(async _ =>
    {
        SelectedDate = null;
        await UpdateSessionsAsync();
    });

    public ICommand ToSessionCommand => new Command(_ =>
    {
        if (SelectedSession is null)
        {
            return;
        }

        SessionNavigator.NavigateWithBar(this, navigationStore, Patient, SelectedSession);
    });

    public ICommand DeleteSessionCommand => new AsyncCommand(async _ =>
    {
        if (SelectedSession is null)
        {
            return;
        }

        _ = database.Sessions.Remove(SelectedSession);
        _ = await database.SaveChangesAsync();
        _ = Sessions.Remove(SelectedSession);
        OnPropertyChanged(nameof(Sessions));

        if (Sessions.Count == 0 && _currPage > 0)
        {
            _currPage--;
        }
        IsPreviousEnabled = _currPage > 0;
        IsNextEnabled = _currPage < TotalPages - 1;
        SelectedDate = null;
        await UpdateSessionsAsync();
    });

    public ICommand NextPageCommand => new AsyncCommand(async _ =>
    {
        _currPage++;
        IsPreviousEnabled = true;
        IsNextEnabled = _currPage < TotalPages - 1;
        await UpdateSessionsAsync();
    });

    public ICommand PrevPageCommand => new AsyncCommand(async _ =>
    {
        _currPage--;
        IsPreviousEnabled = _currPage > 0;
        IsNextEnabled = true;
        await UpdateSessionsAsync();
    });

    private async Task UpdateSessionsAsync()
    {
        var query = database.Sessions
            .Where(a => a.Patient == Patient.Id)
            .Include(a => a.MapNavigation)
            .OrderByDescending(a => a.Id)
            .AsQueryable();

        if (SelectedDate is not null)
        {
            query = query.Where(a => a.Date == SelectedDate.Value.Date.ToString("dd.MM.yyyy"));
        }

        Sessions =
        [..
            await query
                .Skip(SessionsPerPage * (_currPage - 1))
                .Take(SessionsPerPage)
                .ToListAsync()
        ];
    }

    public override void Refresh()
    {
        base.Refresh();

        _ = Application.Current.Dispatcher.InvokeAsync(UpdateSessionsAsync).Task.ContinueWith(e =>
        {
            if (e.Exception is not null)
            {
                Log.Error($"{e.Exception.Message} \n {e.Exception.StackTrace}");
            }
        });

        SelectedSession = null;
        HoveredSession = null;
    }
}
