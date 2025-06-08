using System.Windows.Input;

using Disk.Navigators;
using Disk.Stores;
using Disk.ViewModels.Common.Commands.Sync;
using Disk.ViewModels.Common.ViewModels;

namespace Disk.ViewModels;

public class NavigationBarLayoutViewModel(NavigationStore navigationStore) : ObserverViewModel
{
    public bool CanNavigateBack => navigationStore.CanClose;
    public bool CanNavigateToPatients => CurrentViewModel is not PatientsViewModel;
    public bool CanNavigateToSettings => CurrentViewModel is not SettingsViewModel;
    public bool CanNavigateToCalibration => CurrentViewModel is not CalibrationViewModel;
    public bool CanNavigateToMapCreator => CurrentViewModel is not MapCreatorViewModel;

    private ObserverViewModel? _currentViewModel;
    public ObserverViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            value?.Refresh();
            _ = SetProperty(ref _currentViewModel, value);

            OnPropertyChanged(nameof(CanNavigateBack));
            OnPropertyChanged(nameof(CanNavigateToPatients));
            OnPropertyChanged(nameof(CanNavigateToSettings));
            OnPropertyChanged(nameof(CanNavigateToCalibration));
            OnPropertyChanged(nameof(CanNavigateToMapCreator));
        }
    }

    public ICommand NavigateBackCommand => new Command(_ =>
    {
        CurrentViewModel?.BeforeNavigation();
        navigationStore.Close();
        CurrentViewModel?.AfterNavigation();
    });
    public ICommand NavigateToPatientsCommand => new Command(_ =>
        PatientsNavigator.NavigateWithBar(CurrentViewModel ?? this, navigationStore));
    public ICommand NavigateToSettingsCommand => new Command(_ =>
        SettingsNavigator.NavigateWithBar(CurrentViewModel ?? this, navigationStore));
    public ICommand NavigateToCalibrationCommand => new Command(_ =>
        CalibrationNavigator.NavigateWithBar(CurrentViewModel ?? this, navigationStore));
    public ICommand NavigateToMapCreatorCommand => new Command(_ =>
        MapCreatorNavigator.Navigate(CurrentViewModel ?? this, navigationStore));

    public override void Refresh()
    {
        base.Refresh();

        CurrentViewModel?.Refresh();
    }

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);

        CurrentViewModel?.Dispose();
    }

    public override void BeforeNavigation()
    {
        base.BeforeNavigation();

        CurrentViewModel?.BeforeNavigation();
    }

    public override void AfterNavigation()
    {
        base.AfterNavigation();

        CurrentViewModel?.AfterNavigation();
    }
}
