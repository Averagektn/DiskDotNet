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

public class StartSessionViewModel : ObserverViewModel
{
    public required Patient Patient { get; set; }
    public required Appointment Appointment { get; set; }
    public event Action? OnSessionOver;

    private ObservableCollection<Map> _maps = [];
    public ObservableCollection<Map> Maps
    {
        get => _maps;
        set => SetProperty(ref _maps, value);
    }

    public Map? SelectedMap { get; set; }

    public ICommand StartSessionCommand => new Command(StartSession);
    public ICommand CancelCommand => new Command(_ => IniNavigationStore.Close());

    private static Settings Settings => Settings.Default;

    private readonly NavigationStore _navigationStore;
    private readonly ISessionRepository _sessionRepository;
    private readonly IMapRepository _mapRepository;

    public StartSessionViewModel(NavigationStore navigationStore, ISessionRepository sessionRepository, IMapRepository mapRepository)
    {
        _navigationStore = navigationStore;
        _sessionRepository = sessionRepository;
        _mapRepository = mapRepository;

        Maps = [.. _mapRepository.GetAll()];
    }

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
        _sessionRepository.Add(session);

        IniNavigationStore.Close();
        PaintNavigator.Navigate(_navigationStore, Settings.CursorFilePath, logPath, OnSessionOver, session);
        Application.Current.MainWindow.WindowState = WindowState.Maximized;
    }

    public void FilterMapNames(string filter)
    {
        if (filter == string.Empty)
        {
            Maps = [.. _mapRepository.GetAll()];
        }

        var filteredMaps = Maps
            .Where(map => map.Name.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase))
            .Union(Maps.Where(map => map.Name.Contains(filter, StringComparison.CurrentCultureIgnoreCase)))
            .Distinct()
            .ToList();

        Maps = [.. filteredMaps];
    }
}
