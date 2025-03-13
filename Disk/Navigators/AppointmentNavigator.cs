using Disk.Entities;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;
using Disk.ViewModel.Common.ViewModels;

namespace Disk.Navigators;

public class AppointmentNavigator : INavigator
{
    public static void Navigate(ObserverViewModel currentViewModel, INavigationStore navigationStore, Patient patient,
        Appointment appointment)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<AppointmentViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.Appointment = appointment;
            vm.Patient = patient;
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore, Patient patient,
        Appointment appointment)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            Navigate(currentViewModel, navigationStore, patient, appointment);
        }
    }

    public static void NavigateWithBar(ObserverViewModel currentViewModel, INavigationStore navigationStore, Patient patient, Appointment appointment)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<AppointmentViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.Appointment = appointment;
                vm.Patient = patient;
            });
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateWithBarAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore, 
        Patient patient, Appointment appointment)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            NavigateWithBar(currentViewModel, navigationStore, patient, appointment);
        }
    }
}
