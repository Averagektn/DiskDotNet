using Disk.Db.Context;
using Disk.Entities;
using Disk.Navigators;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Localization = Disk.Properties.Langs.ConfigureAppointment.ConfigureAppointmentLocalization;

namespace Disk.ViewModel;

public class ConfigureAppointmentViewModel : PopupViewModel
{
    public required Patient Patient { get; set; }

    // Binding
    private Map? _selectedMap = null;
    public Map? SelectedMap { get => _selectedMap; set => SetProperty(ref _selectedMap, value); }
    public Visibility MapVisibility => SelectedMap is null ? Visibility.Hidden : Visibility.Visible;

    private ObservableCollection<Map> _maps = [];
    public ObservableCollection<Map> Maps
    {
        get => _maps;
        set => SetProperty(ref _maps, value);
    }

    private readonly NavigationStore _navigationStore;
    private readonly DiskContext _database;
    public ConfigureAppointmentViewModel(NavigationStore navigationStore, DiskContext database)
    {
        _navigationStore = navigationStore;
        _database = database;

        _ = Task.Run(UpdateMapsAsync);
    }

    public ICommand CreateAppointmentCommand => new AsyncCommand(async _ =>
    {
        if (SelectedMap is null)
        {
            return;
        }

        var appointment = new Appointment()
        {
            Map = SelectedMap.Id,
            Patient = Patient.Id,
        };

        _ = await _database.Appointments.AddAsync(appointment);

        IniNavigationStore.Close();
        AppointmentNavigator.NavigateWithBar(_navigationStore, Patient, appointment);
    });

    public ICommand DeleteMapCommand => new AsyncCommand(async map =>
    {
        if (map is Map m)
        {
            try
            {
                _ = _database.Maps.Remove(m);
                _ = Maps.Remove(m);
            }
            catch (InvalidOperationException)
            {
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

        _ = Task.Run(UpdateMapsAsync);
    }

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);

        _ = Task.Run(async () => await _database.SaveChangesAsync());
    }
}
