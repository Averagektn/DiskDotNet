using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;
using Disk.ViewModel.Common.ViewModels;

namespace Disk.Navigators.Common;

public abstract class NavigatorBase<TViewModel> : INavigator where TViewModel : ObserverViewModel
{
    public static void Navigate(INavigationStore navigationStore)
    {
        navigationStore.SetViewModel<TViewModel>(vm => vm.IniNavigationStore = navigationStore);
    }

    public static void NavigateAndClose(INavigationStore navigationStore)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            Navigate(navigationStore);
        }
    }

    public static void NavigateWithBar(INavigationStore navigationStore)
    {
        if (navigationStore.CurrentViewModel is NavigationBarLayoutViewModel bar)
        {
            bar.CurrentViewModel = navigationStore.GetViewModel<TViewModel>(vm => vm.IniNavigationStore = navigationStore);
        }
        else
        {
            navigationStore.SetViewModel<NavigationBarLayoutViewModel>(
                vm =>
                {
                    vm.IniNavigationStore = navigationStore;
                    vm.CurrentViewModel = navigationStore.GetViewModel<TViewModel>(vm => vm.IniNavigationStore = navigationStore);
                });
        }
    }

    public static void NavigateWithBarAndClose(INavigationStore navigationStore)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            NavigateWithBar(navigationStore);
        }
    }
}
