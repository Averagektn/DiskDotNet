using Disk.Entities;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;

namespace Disk.Navigators;

public class AppointmentNavigator : INavigator
{
    public static void Navigate(INavigationStore navigationStore, List<Session> sessions, Patient patient, Appointment appointment)
    {
        navigationStore.SetViewModel<AppointmentViewModel>(
            vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.Appointment = appointment;
                vm.Patient = patient;
                vm.Sessions = [.. sessions];
            });
    }

    public static void NavigateAndClose(INavigationStore navigationStore, List<Session> sessions, Patient patient, 
        Appointment appointment)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            Navigate(navigationStore, sessions, patient, appointment);
        }
    }

    public static void NavigateWithBar(INavigationStore navigationStore, List<Session> sessions, Patient patient, 
        Appointment appointment)
    {
        if (navigationStore.CurrentViewModel is NavigationBarLayoutViewModel bar)
        {
            bar.CurrentViewModel = navigationStore.GetViewModel<AppointmentViewModel>(vm =>
            {
                vm.Appointment = appointment;
                vm.Patient = patient;
                vm.Sessions = [.. sessions];
            });
        }
        else
        {
            navigationStore.SetViewModel<NavigationBarLayoutViewModel>(
                vm =>
                {
                    vm.IniNavigationStore = navigationStore;
                    vm.CurrentViewModel = navigationStore.GetViewModel<AppointmentViewModel>(vm =>
                    {
                        vm.Appointment = appointment;
                        vm.Patient = patient;
                        vm.Sessions = [.. sessions];
                    });
                });
        }
    }

    public static void NavigateWithBarAndClose(INavigationStore navigationStore, List<Session> sessions, Patient patient, 
        Appointment appointment)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            NavigateWithBar(navigationStore, sessions, patient, appointment);
        }
    }
}
