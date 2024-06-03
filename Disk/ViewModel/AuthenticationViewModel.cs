using Disk.Entities;
using Disk.Stores;
using Disk.ViewModel.Common;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class AuthenticationViewModel : PopupViewModel
    {
        public Doctor Doctor { get; set; } = new();

        private readonly NavigationStore _navigationStore;
        public ICommand NavigateBackCommand { get; set; }

        public AuthenticationViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            NavigateBackCommand = new Command(_ => _navigationStore.CurrentViewModel = new MapCreatorViewModel());
        }
    }
}