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

        public void CloseModal()
        {
            _modalNavigationStore.Close();
        }

        public override void Dispose()
        {
            base.Dispose();
            GC.SuppressFinalize(this);

            while (_navigationStore.CanClose) 
            {
                _navigationStore.Close();
            }
            while (_modalNavigationStore.CanClose) 
            { 
                _modalNavigationStore.Close(); 
            }
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