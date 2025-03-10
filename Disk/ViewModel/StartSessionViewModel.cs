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
using Localization = Disk.Properties.Langs.StartSession.StartSessionLocalization;

namespace Disk.ViewModel;

public class StartSessionViewModel : ObserverViewModel
{
    public required Patient Patient { get; set; }
    public required Appointment Appointment { get; set; }
    
    // Binding
    public Map? SelectedMap { get; set; }
    public Visibility MapVisibility => SelectedMap is null ? Visibility.Hidden : Visibility.Visible;
    public static string MaxXAngle => $"{Settings.XMaxAngle:f2}";
    public static string MaxYAngle => $"{Settings.YMaxAngle:f2}";
    public static string Ip => Settings.IP;
    public static string CursorImageName => Path.GetFileName(Settings.CursorFilePath);
    public static string CursorImagePath => Settings.CursorFilePath;
    public static int ShotFrequency => 1000 / Settings.ShotTime;
    public static int TargetLifespan => 1000 * Settings.TargetHp / ShotFrequency;
    private static Settings Settings => Settings.Default;

    private ObservableCollection<Map> _maps = [];
    public ObservableCollection<Map> Maps
    {
        get => _maps;
        set => SetProperty(ref _maps, value);
    }

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

    public ICommand StartSessionCommand => new Command(_ =>
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
            MaxXAngle = Settings.XMaxAngle,
            MaxYAngle = Settings.YMaxAngle,
            CursorRadius = Settings.IniUserRadius,
            TargetRadius = Settings.IniTargetRadius,
        };
        _sessionRepository.Add(session);

        IniNavigationStore.Close();
        PaintNavigator.Navigate(_navigationStore, session);
        Application.Current.MainWindow.WindowState = WindowState.Maximized;
    });

    public ICommand DeleteMapCommand => new Command(map =>
    {
        if (map is Map m)
        {
            try
            {
                _mapRepository.Delete(m);
                Maps.Remove(m);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(Localization.MapIsInUseError);
            }
        }
    });

    public ICommand MapSelectedCommand => new Command(_ => OnPropertyChanged(nameof(MapVisibility)));

    public override void Refresh()
    {
        base.Refresh();

        Maps = [.. _mapRepository.GetAll()];
    }
}
