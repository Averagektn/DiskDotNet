using Disk.Entities;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;

namespace Disk.Navigators;

public class AppointmentsListNavigator : INavigator
{
    public static void Navigate(INavigationStore navigationStore, Patient patient)
    {
        navigationStore.SetViewModel<AppointmentsListViewModel>(
            vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.Patient = patient;
            });
    }

    public static void NavigateAndClose(INavigationStore navigationStore, Patient patient)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            Navigate(navigationStore, patient);
        }
    }

    public static void NavigateWithBar(INavigationStore navigationStore, Patient patient)
    {
        if (navigationStore.CurrentViewModel is NavigationBarLayoutViewModel bar)
        {
            bar.CurrentViewModel = navigationStore.GetViewModel<AppointmentsListViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.Patient = patient;
            });
        }
        else
        {
            navigationStore.SetViewModel<NavigationBarLayoutViewModel>(
                vm =>
                {
                    vm.IniNavigationStore = navigationStore;
                    vm.CurrentViewModel = navigationStore.GetViewModel<AppointmentsListViewModel>(vm =>
                    {
                        vm.IniNavigationStore = navigationStore;
                        vm.Patient = patient;
                    });
                });
        }
    }

    public static void NavigateWithBarAndClose(INavigationStore navigationStore, Patient patient)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            NavigateWithBar(navigationStore, patient);
        }
    }
}
