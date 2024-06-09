using Disk.Stores;

namespace Disk.ViewModel.Common.ViewModels
{
    public class MainViewModel : ObserverViewModel
    {
        public ObserverViewModel CurrentViewModel => _navigationStore.CurrentViewModel;
        public ObserverViewModel? CurrentModalViewModel => _modalNavigationStore.CurrentViewModel;

        private readonly NavigationStore _navigationStore;
        private readonly ModalNavigationStore _modalNavigationStore;

        public bool IsModalOpen => _modalNavigationStore.IsOpen;

        public MainViewModel(NavigationStore navigationStore, ModalNavigationStore modalNavigationStore)
        {
            _navigationStore = navigationStore;
            _modalNavigationStore = modalNavigationStore;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _modalNavigationStore.CurrentViewModelChanged += OnCurrentModalViewModelChanged;
        }

        public bool CanNavigateBack()
        {
            if (_modalNavigationStore.CanClose)
            {
                _modalNavigationStore.Close();
                return true;
            }
            return _navigationStore.NavigateBack();
        }

        private void OnCurrentModalViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentModalViewModel));
            OnPropertyChanged(nameof(IsModalOpen));
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}