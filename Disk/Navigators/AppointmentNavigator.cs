using Disk.Entities;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;

namespace Disk.Navigators;

public class AppointmentNavigator : INavigator
{
    public static void Navigate(INavigationStore navigationStore, Patient patient, Appointment appointment)
    {
        navigationStore.SetViewModel<AppointmentViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.Appointment = appointment;
            vm.Patient = patient;
        });
    }

    public static void NavigateAndClose(INavigationStore navigationStore, Patient patient, Appointment appointment)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            Navigate(navigationStore, patient, appointment);
        }
    }

    public static void NavigateWithBar(INavigationStore navigationStore, Patient patient, Appointment appointment)
    {
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<AppointmentViewModel>(vm =>
            {
                vm.Appointment = appointment;
                vm.Patient = patient;
            });
        });
    }

    public static void NavigateWithBarAndClose(INavigationStore navigationStore, Patient patient, Appointment appointment)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            NavigateWithBar(navigationStore, patient, appointment);
        }
    }
}
