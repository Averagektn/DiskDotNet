using Disk.Stores;
using Disk.ViewModel.Common;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class AuthenticationViewModel : ObserverViewModel
    {
        private readonly NavigationStore _navigationStore;
        public ICommand NavigateBackCommand { get; set; }

        public AuthenticationViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            NavigateBackCommand = new Command(_ => _navigationStore.CurrentViewModel = new MapViewModel());
        }
    }
}