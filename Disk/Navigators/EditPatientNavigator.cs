using Disk.Entities;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;
using Newtonsoft.Json;
using System.Globalization;

namespace Disk.Navigators;

public class EditPatientNavigator : INavigator
{
    public static void Navigate(INavigationStore navigationStore, Patient patient)
    {
        navigationStore.SetViewModel<EditPatientViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.Patient = JsonConvert.DeserializeObject<Patient>(JsonConvert.SerializeObject(patient))!;
            vm.Patient = patient;
            vm.DateOfBirth = DateTime.ParseExact(patient.DateOfBirth, "dd.MM.yyyy", CultureInfo.InvariantCulture);
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
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<EditPatientViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.Patient = JsonConvert.DeserializeObject<Patient>(JsonConvert.SerializeObject(patient))!;
                vm.Patient = patient;
            });
        });
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
