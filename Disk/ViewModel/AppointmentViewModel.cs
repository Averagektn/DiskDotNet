﻿using Disk.Entities;
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
    public Session? SelectedSession { get; set; } = null;

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

        PathsToTargets = [.. SelectedSession.PathToTargets];
    }

    private void Update()
    {
        Sessions = [.. sessionRepository.GetSessionsWithResultsByAppointment(Appointment.Id)];
    }
}
