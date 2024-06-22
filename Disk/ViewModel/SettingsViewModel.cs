using Disk.Stores;
using Disk.ViewModel.Common.ViewModels;

namespace Disk.ViewModel
{
    public class SettingsViewModel(NavigationStore navigationStore) : ObserverViewModel
    {
        public void Close()
        {
            _ = navigationStore.NavigateBack();
        }
    }
}