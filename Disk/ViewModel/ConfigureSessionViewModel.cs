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
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Localization = Disk.Properties.Langs.ConfigureSession.ConfigureSessionLocalization;

namespace Disk.ViewModel;

public class ConfigureSessionViewModel : PopupViewModel
{
    public required Patient Patient { get; set; }

    // Binding
    private Map? _selectedMap = null;
    public Map? SelectedMap
    {
        get => _selectedMap;
        set
        {
            _ = SetProperty(ref _selectedMap, value);
            OnPropertyChanged(nameof(IsCreateSessionEnabled));
        }
    }
    public Visibility MapVisibility => SelectedMap is null ? Visibility.Hidden : Visibility.Visible;
    public bool IsCreateSessionEnabled => SelectedMap is not null;

    private ObservableCollection<Map> _maps = [];
    public ObservableCollection<Map> Maps { get => _maps; set => SetProperty(ref _maps, value); }

    private readonly NavigationStore _navigationStore;
    private readonly DiskContext _database;
    public ConfigureSessionViewModel(NavigationStore navigationStore, DiskContext database)
    {
        _navigationStore = navigationStore;
        _database = database;

        UpdateMapsAsync().Wait();
    }

    public ICommand CreateSessionCommand => new AsyncCommand(async _ =>
    {
        if (SelectedMap is null)
        {
            return;
        }

        var session = new Session()
        {
            Map = SelectedMap.Id,
            Patient = Patient.Id,
            Date = DateTime.Now.Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
        };

        try
        {
            _ = await _database.Sessions.AddAsync(session);
            _ = await _database.SaveChangesAsync();
        }
        catch
        {
            await ShowPopup(header: Localization.Map, message: Localization.DuplicatePatientDateMap);
        }

        IniNavigationStore.Close();
        SessionNavigator.NavigateWithBar(this, _navigationStore, Patient, session);
    });

    public ICommand DeleteMapCommand => new AsyncCommand(async map =>
    {
        if (map is Map m)
        {
            try
            {
                _ = _database.Maps.Remove(m);
                _ = await _database.SaveChangesAsync();
                _ = Maps.Remove(m);
            }
            catch (Exception ex)
            {
                Log.Error($"Map deletion error {ex.Message} {ex.StackTrace}");
                await ShowPopup(header: Localization.Map, message: Localization.MapIsInUseError);
            }
        }
    });

    public ICommand MapSelectedCommand => new Command(_ => OnPropertyChanged(nameof(MapVisibility)));

    public async Task UpdateMapsAsync()
    {
        Maps = [.. await _database.Maps.ToListAsync()];
    }

    public override void Refresh()
    {
        base.Refresh();

        _ = Application.Current.Dispatcher.InvokeAsync(UpdateMapsAsync);
    }
}
