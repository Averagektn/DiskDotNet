using Disk.Stores;

namespace Disk.ViewModel.Common.ViewModels
{
    public class MainViewModel : ObserverViewModel
    {
        public ObserverViewModel CurrentViewModel => _navigationStore.CurrentViewModel;

        private readonly NavigationStore _navigationStore;

        public MainViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        public bool CanNavigateBack() => _navigationStore.NavigateBack();

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}