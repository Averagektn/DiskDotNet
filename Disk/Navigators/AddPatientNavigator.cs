using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;

namespace Disk.Navigators;

public class AddPatientNavigator : INavigator
{
    public static void Navigate(INavigationStore navigationStore)
    {
        navigationStore.SetViewModel<AddPatientViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
        });
    }

    public static void NavigateAndClose(INavigationStore navigationStore)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
        }
    }

    public static void NavigateWithBar(INavigationStore navigationStore)
    {
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<AddPatientViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
            });
        });
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
