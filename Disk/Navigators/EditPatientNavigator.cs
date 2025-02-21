using Disk.Entities;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;
using Newtonsoft.Json;
using System.Globalization;

namespace Disk.Navigators;

public class EditPatientNavigator : INavigator
{
    public static void Navigate(INavigationStore navigationStore, Action? afterUpdateEvent, Patient patient)
    {
        navigationStore.SetViewModel<EditPatientViewModel>(
            vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.Patient = JsonConvert.DeserializeObject<Patient>(JsonConvert.SerializeObject(patient))!;
                vm.AttachedPatient = patient;
                vm.DateOfBirth = DateTime.ParseExact(patient.DateOfBirth, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                vm.AfterUpdateEvent += afterUpdateEvent;
            });
    }

    public static void NavigateAndClose(INavigationStore navigationStore, Action? afterUpdateEvent, Patient patient)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            Navigate(navigationStore, afterUpdateEvent, patient);
        }
    }

    public static void NavigateWithBar(INavigationStore navigationStore, Action? afterUpdateEvent, Patient patient)
    {
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(
            vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.CurrentViewModel = navigationStore.GetViewModel<EditPatientViewModel>(
                    vm =>
                    {
                        vm.IniNavigationStore = navigationStore;
                        vm.Patient = JsonConvert.DeserializeObject<Patient>(JsonConvert.SerializeObject(patient))!;
                        vm.AttachedPatient = patient;
                        vm.AfterUpdateEvent += afterUpdateEvent;
                    });
            });
    }

    public static void NavigateWithBarAndClose(INavigationStore navigationStore, Action? afterUpdateEvent, Patient patient)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            NavigateWithBar(navigationStore, afterUpdateEvent, patient);
        }
    }
}
