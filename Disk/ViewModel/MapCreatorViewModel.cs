using Disk.Navigators;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Disk.Visual.Impl;
using System.Windows.Input;

namespace Disk.ViewModel;

public class MapCreatorViewModel(ModalNavigationStore modalNavigationStore) : ObserverViewModel
{
    public virtual ICommand CancelCommand => new Command(_ => IniNavigationStore.Close());

    public void SaveMap(List<NumberedTarget> targets)
    {
        if (targets.Count != 0)
        {
            var map = targets
                .Select(t => t.Angles)
                .ToList();

            MapNamePickerNavigator.Navigate(modalNavigationStore, map);
        }

        IniNavigationStore.Close();
    }
}