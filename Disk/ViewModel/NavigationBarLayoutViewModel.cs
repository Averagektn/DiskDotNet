using Disk.Navigators;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Windows.Input;

namespace Disk.ViewModel;

public class NavigationBarLayoutViewModel(NavigationStore navigationStore) : ObserverViewModel
{
    public readonly Stack<Type> ViewModels = [];
    public ICommand NavigateBackCommand => new Command(_ =>
    {
        if (ViewModels.Count > 1)
        {
            CurrentViewModel?.Dispose();
            _ = ViewModels.Pop();

            OnPropertyChanged(nameof(CurrentViewModel));
            OnPropertyChanged(nameof(CanNavigateBack));
            OnPropertyChanged(nameof(CanNavigateToPatients));
            OnPropertyChanged(nameof(CanNavigateToSettings));
            OnPropertyChanged(nameof(CanNavigateToCalibration));
            OnPropertyChanged(nameof(CanNavigateToMapCreator));
        }
        else
        {
            navigationStore.Close();
        }
    });
    public ICommand NavigateToPatientsCommand => new Command(_ => PatientsNavigator.NavigateWithBar(navigationStore));
    public ICommand NavigateToSettingsCommand => new Command(_ => SettingsNavigator.NavigateWithBar(navigationStore));
    public ICommand NavigateToCalibrationCommand => new Command(_ => CalibrationNavigator.NavigateWithBar(navigationStore));
    public ICommand NavigateToMapCreatorCommand => new Command(_ => MapCreatorNavigator.Navigate(navigationStore));

    public bool CanNavigateBack => ViewModels.Count > 1;
    public bool CanNavigateToPatients => CurrentViewModel is not PatientsViewModel;
    public bool CanNavigateToSettings => CurrentViewModel is not SettingsViewModel;
    public bool CanNavigateToCalibration => CurrentViewModel is not CalibrationViewModel;
    public bool CanNavigateToMapCreator => CurrentViewModel is not MapCreatorViewModel;

    public required ObserverViewModel? CurrentViewModel
    {
        get
        {
            if (ViewModels.TryPeek(out var res))
            {
                return navigationStore.GetViewModel(res);
            }
            return null;
        }
        set
        {
            if (value is null)
            {
                return;
            }

            ViewModels.Push(value.GetType());

            OnPropertyChanged(nameof(CurrentViewModel));
            OnPropertyChanged(nameof(CanNavigateBack));
            OnPropertyChanged(nameof(CanNavigateToPatients));
            OnPropertyChanged(nameof(CanNavigateToSettings));
            OnPropertyChanged(nameof(CanNavigateToCalibration));
            OnPropertyChanged(nameof(CanNavigateToMapCreator));
        }
    }
}
