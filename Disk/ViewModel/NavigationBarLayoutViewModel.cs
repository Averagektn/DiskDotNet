using Disk.Navigators;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Windows.Input;

namespace Disk.ViewModel;

public class NavigationBarLayoutViewModel(NavigationStore navigationStore) : ObserverViewModel
{
    public ICommand NavigateBackCommand => new Command(_ => navigationStore.Close());
    public ICommand NavigateToPatientsCommand => new Command(_ => PatientsNavigator.NavigateWithBar(navigationStore));
    public ICommand NavigateToSettingsCommand => new Command(_ => SettingsNavigator.NavigateWithBar(navigationStore));
    public ICommand NavigateToCalibrationCommand => new Command(_ => CalibrationNavigator.NavigateWithBar(navigationStore));
    public ICommand NavigateToMapCreatorCommand => new Command(_ => MapCreatorNavigator.Navigate(navigationStore));

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
            SetProperty(ref _currentViewModel, value);
        }
    }

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
}
