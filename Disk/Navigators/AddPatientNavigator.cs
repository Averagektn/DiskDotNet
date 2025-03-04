using Disk.Entities;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;

namespace Disk.Navigators;

public class AddPatientNavigator : INavigator
{
    public static void Navigate(INavigationStore navigationStore, Action<Patient>? onAddEvents = null)
    {
        navigationStore.SetViewModel<AddPatientViewModel>(vm =>
        {
            vm.OnAddEvent += onAddEvents;
            vm.IniNavigationStore = navigationStore;
        });
    }

    public static void NavigateAndClose(INavigationStore navigationStore, Action<Patient>? onAddEvents = null)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            Navigate(navigationStore, onAddEvents);
        }
    }

    public static void NavigateWithBar(INavigationStore navigationStore, Action<Patient>? onAddEvents = null)
    {
        if (navigationStore.CurrentViewModel is NavigationBarLayoutViewModel bar)
        {
            bar.CurrentViewModel = navigationStore.GetViewModel<AddPatientViewModel>(vm =>
            {
                vm.OnAddEvent += onAddEvents;
                vm.IniNavigationStore = navigationStore;
            });
        }
        else
        {
            navigationStore.SetViewModel<NavigationBarLayoutViewModel>(
                vm =>
                {
                    vm.IniNavigationStore = navigationStore;
                    vm.CurrentViewModel = navigationStore.GetViewModel<AddPatientViewModel>(vm =>
                    {
                        vm.OnAddEvent += onAddEvents;
                        vm.IniNavigationStore = navigationStore;
                    });
                });
        }
    }

    public static void NavigateWithBarAndClose(INavigationStore navigationStore, Action<Patient>? onAddEvents = null)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            NavigateWithBar(navigationStore, onAddEvents);
        }
    }
}
