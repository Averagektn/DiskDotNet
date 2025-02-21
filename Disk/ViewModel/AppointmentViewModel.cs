using Disk.Entities;
using Disk.Navigators;
using Disk.Repository.Interface;
using Disk.Service.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Disk.ViewModel;

public class AppointmentViewModel(ModalNavigationStore modalNavigationStore, ISessionRepository sessionRepository, 
    IExcelFiller excelFiller, NavigationStore navigationStore) : ObserverViewModel
{
    public required Patient Patient { get; set; }
    public required Appointment Appointment { get; set; }
    public Session? SelectedSession { get; set; } = null;
    public ObservableCollection<Session> Sessions { get; set; } = [];
    public ObservableCollection<PathToTarget> PathsToTargets { get; set; } = [];

    public ICommand StartSessionCommand => 
        new Command(_ => StartSessionNavigator.Navigate(modalNavigationStore, Update, Appointment, Patient));
    public ICommand SessionSelectedCommand => new Command(SessionSelected);
    public ICommand ExportToExcelCommand => new Command(_ => excelFiller.ExportToExcel(Sessions, Patient));
    public ICommand ShowSessionCommand => new Command(ShowSession);
    public ICommand DeleteSessionCommand => new Command(_ =>
    {
        sessionRepository.Delete(SelectedSession!);
        _ = Sessions.Remove(SelectedSession!);
        OnPropertyChanged(nameof(Sessions));
        PathsToTargets.Clear();

        SelectedSession = null;
    });

    private void ShowSession(object? obj)
    {
        if (SelectedSession is null)
        {
            return;
        }

        SessionResultNavigator.Navigate(navigationStore, SelectedSession);
        Application.Current.MainWindow.WindowState = WindowState.Maximized;
    }

    private void SessionSelected(object? obj)
    {
        if (SelectedSession is null)
        {
            return;
        }

        PathsToTargets.Clear();

        foreach (var pathToTarget in SelectedSession.PathToTargets)
        {
            PathsToTargets.Add(pathToTarget);
        }
    }

    private void Update()
    {
        Sessions.Clear();
        var sessions = sessionRepository.GetSessionsWithResultsByAppointment(Appointment.Id);

        foreach (var session in sessions)
        {
            Sessions.Add(session);
        }
    }
}
