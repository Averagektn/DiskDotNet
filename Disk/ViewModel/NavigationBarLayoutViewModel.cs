using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Security.Policy;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class NavigationBarLayoutViewModel(NavigationStore navigationStore) : ObserverViewModel
    {
        public ICommand NavigateBackCommand => new Command(_ => navigationStore.NavigateBack());
        public ICommand NavigateToPatientsCommand => new Command(
            _ => navigationStore.SetViewModel<NavigationBarLayoutViewModel>(
                vm => vm.CurrentViewModel = navigationStore.GetViewModel<PatientsViewModel>()
                )
            );
        public ICommand NavigateToSettingsCommand => new Command(
            _ => navigationStore.SetViewModel<NavigationBarLayoutViewModel>(
                vm => vm.CurrentViewModel = navigationStore.GetViewModel<SettingsViewModel>()
                )
            );
        public ICommand NavigateToCalibrationCommand => new Command(
            _ => navigationStore.SetViewModel<NavigationBarLayoutViewModel>(
                vm => vm.CurrentViewModel = navigationStore.GetViewModel<CalibrationViewModel>()
                )
            );
        public ICommand NavigateToMapCreatorCommand => new Command(_ => navigationStore.SetViewModel<MapCreatorViewModel>());

        public bool CanNavigateBack => navigationStore.CanNavigateBack;
        public bool CanNavigateToPatients => CurrentViewModel is not PatientsViewModel;
        public bool CanNavigateToSettings => CurrentViewModel is not SettingsViewModel;
        public bool CanNavigateToCalibration => CurrentViewModel is not CalibrationViewModel;
        public bool CanNavigateToMapCreator => CurrentViewModel is not MapCreatorViewModel;

        private ObserverViewModel? _currentViewModel;
        public ObserverViewModel? CurrentViewModel { get => _currentViewModel; set => SetProperty(ref _currentViewModel, value); }
    }
}
