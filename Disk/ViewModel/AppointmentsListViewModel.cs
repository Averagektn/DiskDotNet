using Disk.Db.Context;
using Disk.Entities;
using Disk.Navigators;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Disk.ViewModel;

public class AppointmentsListViewModel(DiskContext database, NavigationStore navigationStore) : ObserverViewModel
{
    private const int AppointmentsPerPage = 15;
    private int _currPage;
    public Appointment? SelectedAppointment { get; set; }

    private Patient _patient = null!;
    public required Patient Patient
    {
        get => _patient;
        set
        {
            _patient = value;

            _ = Task.Run(async () =>
            {
                await UpdateAppointmentsAsync();

                IsNextEnabled = _currPage < TotalPages - 1;
            });
        }
    }

    private ObservableCollection<Appointment> _appointments = [];
    public ObservableCollection<Appointment> Appointments
    {
        get => _appointments;
        set => SetProperty(ref _appointments, value);
    }

    private DateTime? _selectedDate;
    public DateTime? SelectedDate { get => _selectedDate; set => SetProperty(ref _selectedDate, value); }

    private bool _isNextEnabled;
    public bool IsNextEnabled { get => _isNextEnabled; set => SetProperty(ref _isNextEnabled, value); }

    private bool _isPreviousEnabled;
    public bool IsPreviousEnabled { get => _isPreviousEnabled; set => SetProperty(ref _isPreviousEnabled, value); }

    private int TotalPages
    {
        get
        {
            var appointmentsCount = database.Appointments.Count();

            return (int)Math.Ceiling((double)appointmentsCount / AppointmentsPerPage);
        }
    }

    /*    public ICommand StartAppointmentCommand => new Command(_ => 
            StartSessionNavigator.NavigateWithBar(navigationStore, Patient, appointment));*/

    public ICommand CancelDateCommand => new AsyncCommand(async _ =>
    {
        SelectedDate = null;
        await UpdateAppointmentsAsync();
    });

    public ICommand ToAppointmentCommand => new Command(_ =>
    {
        if (SelectedAppointment is null)
        {
            return;
        }

        AppointmentNavigator.NavigateWithBar(navigationStore, Patient, SelectedAppointment);
    });

    public ICommand DeleteAppointmentCommand => new AsyncCommand(async _ =>
    {
        if (SelectedAppointment is null)
        {
            return;
        }

        _ = database.Appointments.Remove(SelectedAppointment);
        _ = Appointments.Remove(SelectedAppointment);
        OnPropertyChanged(nameof(Appointments));

        if (Appointments.Count == 0 && _currPage > 0)
        {
            _currPage--;
        }

        IsPreviousEnabled = _currPage > 0;
        IsNextEnabled = _currPage < TotalPages - 1;

        SelectedDate = null;
        await UpdateAppointmentsAsync();
    });

    public ICommand NextPageCommand => new AsyncCommand(async _ =>
    {
        _currPage++;
        IsPreviousEnabled = true;
        IsNextEnabled = _currPage < TotalPages - 1;

        SelectedDate = null;

        await UpdateAppointmentsAsync();
    });

    public ICommand PrevPageCommand => new AsyncCommand(async _ =>
    {
        _currPage--;
        IsPreviousEnabled = _currPage > 0;
        IsNextEnabled = true;

        SelectedDate = null;

        await UpdateAppointmentsAsync();
    });

    private async Task UpdateAppointmentsAsync()
    {
        Appointments =
        [..
            await database.Appointments
                .Where(a => a.Patient == Patient.Id)
                .OrderByDescending(a => a.Id)
                .Skip(AppointmentsPerPage * (_currPage - 1))
                .Take(AppointmentsPerPage)
                .ToListAsync()
        ];
    }

    public override void Refresh()
    {
        base.Refresh();

        _ = Task.Run(UpdateAppointmentsAsync);
    }
}
