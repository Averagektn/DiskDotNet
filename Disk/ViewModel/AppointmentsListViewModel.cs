using Disk.Entities;
using Disk.Navigators;
using Disk.Repository.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace Disk.ViewModel;

public class AppointmentsListViewModel(NavigationStore navigationStore, IAppointmentRepository appointmentRepository,
    ISessionRepository sessionRepository) : ObserverViewModel
{
    private const int AppointmentsPerPage = 15;
    public Appointment? SelectedAppointment { get; set; }

    private Patient _patient = null!;
    public required Patient Patient
    {
        get => _patient;
        set
        {
            _patient = value;

            Appointments = [.. appointmentRepository.GetPagedAppointments(Patient.Id, currPage, AppointmentsPerPage)];

            IsNextEnabled = currPage < PagesCount - 1;
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

    private int currPage;
    private int PagesCount => (int)float.Ceiling((float)appointmentRepository.GetAppointmentsCount(Patient.Id) / AppointmentsPerPage);

    public ICommand StartAppointmentCommand => new AsyncCommand(StartAppointmentAsync);
    public ICommand CancelDateCommand => new Command(_ =>
    {
        SelectedDate = null;
        UpdateAppointments();
    });

    public ICommand ToAppointmentCommand => new Command(_ =>
    {
        if (SelectedAppointment is null)
        {
            return;
        }

        var sessions = sessionRepository.GetSessionsWithResultsByAppointment(SelectedAppointment.Id).ToList();
        AppointmentNavigator.NavigateWithBar(navigationStore, sessions, Patient, SelectedAppointment);
    });

    public ICommand DeleteAppointmentCommand => new Command(_ =>
    {
        if (SelectedAppointment is null)
        {
            return;
        }

        appointmentRepository.Delete(SelectedAppointment);
        _ = Appointments.Remove(SelectedAppointment);
        OnPropertyChanged(nameof(Appointments));

        if (Appointments.Count == 0 && currPage > 0)
        {
            currPage--;
        }

        IsPreviousEnabled = currPage > 0;
        IsNextEnabled = currPage < PagesCount - 1;

        SelectedDate = null;
        UpdateAppointments();
    });

    public ICommand NextPageCommand => new Command(_ =>
    {
        currPage++;
        IsPreviousEnabled = true;
        IsNextEnabled = currPage < PagesCount - 1;

        SelectedDate = null;

        UpdateAppointments();
    });

    public ICommand PrevPageCommand => new Command(_ =>
    {
        currPage--;
        IsPreviousEnabled = currPage > 0;
        IsNextEnabled = true;

        SelectedDate = null;

        UpdateAppointments();
    });

    public ICommand SearchByDateCommand => new Command(_ =>
    {
        if (SelectedDate is not null)
        {
            Appointments = [.. appointmentRepository.GetAppoitmentsByDate(Patient.Id, SelectedDate.Value.Date)];
        }
    });

    private void UpdateAppointments()
    {
        Appointments = [.. appointmentRepository.GetPagedAppointments(Patient.Id, currPage, AppointmentsPerPage)];
        OnPropertyChanged(nameof(Appointments));
    }

    private async Task StartAppointmentAsync(object? arg)
    {
        var appointment = new Appointment()
        {
            DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),
            Patient = Patient.Id
        };

        await appointmentRepository.AddAsync(appointment);

        currPage = 0;
        IsPreviousEnabled = false;
        IsNextEnabled = currPage < PagesCount - 1;
        UpdateAppointments();


        var sessions = sessionRepository.GetSessionsWithResultsByAppointment(appointment.Id).ToList();
        AppointmentNavigator.NavigateWithBar(navigationStore, sessions, Patient, appointment);
    }

    public override void Refresh()
    {
        base.Refresh();

        UpdateAppointments();
    }
}
