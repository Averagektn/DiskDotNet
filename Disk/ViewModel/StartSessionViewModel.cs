using Disk.Entities;
using Disk.Navigators;
using Disk.Repository.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel;

public class StartSessionViewModel(NavigationStore navigationStore, ISessionRepository sessionRepository, IMapRepository mapRepository)
    : ObserverViewModel
{
    public required Patient Patient { get; set; }
    public required Appointment Appointment { get; set; }
    public event Action? OnSessionOver;

    public ObservableCollection<Map> Maps => [.. mapRepository.GetAll()];
    public Map? SelectedMap { get; set; }

    public ICommand StartSessionCommand => new Command(StartSession);
    public ICommand CancelCommand => new Command(_ => IniNavigationStore.Close());

    private static Settings Settings => Settings.Default;

    private void StartSession(object? obj)
    {
        if (SelectedMap is null)
        {
            return;
        }

        var logPath = $"{Settings.MainDirPath}{Path.DirectorySeparatorChar}" +
                $"{Patient.Surname} {Patient.Name}{Path.DirectorySeparatorChar}" +
                $"{DateTime.Now:dd.MM.yyyy HH-mm-ss}";

        if (!Directory.Exists(logPath))
        {
            _ = Directory.CreateDirectory(logPath);
        }

        var session = new Session()
        {
            Appointment = Appointment.Id,
            DateTime = DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
            LogFilePath = logPath,
            Map = SelectedMap!.Id,
            MaxXAngle = Settings.XMaxAngle,
            MaxYAngle = Settings.YMaxAngle
        };
        sessionRepository.Add(session);

        IniNavigationStore.Close();
        PaintNavigator.Navigate(navigationStore, Settings.CursorFilePath, logPath, OnSessionOver, session);
        Application.Current.MainWindow.WindowState = WindowState.Maximized;
    }
}
