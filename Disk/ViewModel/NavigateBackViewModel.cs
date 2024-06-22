using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class NavigateBackViewModel(NavigationStore navigationStore) : ObserverViewModel
    {
        public ICommand NavigateBackCommand => new Command(_ => navigationStore.NavigateBack());
        public bool CanNavigateBack => navigationStore.CanNavigateBack;
        private ObserverViewModel? _currentViewModel;
        public ObserverViewModel? CurrentViewModel { get => _currentViewModel; set => SetProperty(ref _currentViewModel, value); }
    }
}
