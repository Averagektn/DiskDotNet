using Disk.Entities;
using Disk.Navigators.Interfaces;
using Disk.Stores.Interfaces;
using Disk.ViewModels;
using Disk.ViewModels.Common.ViewModels;
using Newtonsoft.Json;
using System.Globalization;

namespace Disk.Navigators;

public class EditPatientNavigator : INavigator
{
    public static void Navigate(ObserverViewModel currentViewModel, INavigationStore navigationStore, Patient patient)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<EditPatientViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.Patient = JsonConvert.DeserializeObject<Patient>(JsonConvert.SerializeObject(patient))!;
            vm.Patient = patient;
            vm.DateOfBirth = DateTime.ParseExact(patient.DateOfBirth, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore, Patient patient)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            Navigate(currentViewModel, navigationStore, patient);
        }
    }

    public static void NavigateWithBar(ObserverViewModel currentViewModel, INavigationStore navigationStore, Patient patient)
    {
        currentViewModel.BeforeNavigation();
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
        currentViewModel.AfterNavigation();
    }

    public static void NavigateWithBarAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore, Patient patient)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            NavigateWithBar(currentViewModel, navigationStore, patient);
        }
    }
}
