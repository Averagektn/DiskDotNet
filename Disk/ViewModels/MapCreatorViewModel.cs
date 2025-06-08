using System.Windows.Input;

using Disk.Navigators;
using Disk.Stores;
using Disk.ViewModels.Common.Commands.Sync;
using Disk.ViewModels.Common.ViewModels;
using Disk.Visual.Implementations;

namespace Disk.ViewModels;

public class MapCreatorViewModel(ModalNavigationStore modalNavigationStore) : ObserverViewModel
{
    public void SaveMap(List<NumberedTarget> targets)
    {
        if (targets.Count != 0)
        {
            var map = targets
                .Select(t => t.Angles)
                .ToList();

            MapNamePickerNavigator.Navigate(this, modalNavigationStore, map);
        }

        IniNavigationStore.Close();
    }

    public virtual ICommand CancelCommand => new Command(_ => IniNavigationStore.Close());
}
