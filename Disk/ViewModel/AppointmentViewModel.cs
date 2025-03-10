using Disk.Entities;
using Disk.Navigators;
using Disk.Repository.Interface;
using Disk.Service.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Localization = Disk.Properties.Langs.Appointment.AppointmentLocalization;

namespace Disk.ViewModel;

public class AppointmentViewModel(ISessionRepository sessionRepository, IExcelFiller excelFiller, NavigationStore navigationStore) : 
    PopupViewModel
{
    public required Patient Patient { get; set; }
    public Session? SelectedSession { get; set; } = null;

    private Appointment _appointment = null!;
    public required Appointment Appointment
    {
        get => _appointment;
        set
        {
            SetProperty(ref _appointment, value);
            Sessions = [.. sessionRepository.GetSessionsWithResultsByAppointment(value.Id)];
        }
    }

    private ObservableCollection<Session> _sessions = [];
    public ObservableCollection<Session> Sessions 
    {
        get => _sessions;
        set => SetProperty(ref _sessions, value);
    }

    private ObservableCollection<PathToTarget> _pathsToTargets = [];
    public ObservableCollection<PathToTarget> PathsToTargets 
    { 
        get => _pathsToTargets;
        set => SetProperty(ref _pathsToTargets, value); 
    }

    public ICommand StartSessionCommand => 
        new Command(_ => StartSessionNavigator.NavigateWithBar(navigationStore, Appointment, Patient));
    
    public ICommand SessionSelectedCommand => new Command(SessionSelected);
    
    public ICommand ExportToExcelCommand => new Command(_ => excelFiller.ExportToExcel(Appointment, Patient));

    public ICommand ShowSessionCommand => new AsyncCommand(async _ =>
    {
        if (SelectedSession is null)
        {
            return;
        }

        try
        {
            SessionResultNavigator.Navigate(navigationStore, SelectedSession);
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }
        catch
        {
            await ShowPopup(header: Localization.ErrorCaption, message: Localization.NoContentError);
        }
    });

    public ICommand DeleteSessionCommand => new Command(_ =>
    {
        sessionRepository.Delete(SelectedSession!);
        _ = Sessions.Remove(SelectedSession!);
        OnPropertyChanged(nameof(Sessions));
        PathsToTargets.Clear();

        SelectedSession = null;
    });

    private void SessionSelected(object? obj)
    {
        if (SelectedSession is null)
        {
            return;
        }

        PathsToTargets = [.. SelectedSession.PathToTargets];
    }

    private void Update()
    {
        Sessions = [.. sessionRepository.GetSessionsWithResultsByAppointment(Appointment.Id)];
    }

    public override void Refresh()
    {
        base.Refresh();

        Update();
    }
}
