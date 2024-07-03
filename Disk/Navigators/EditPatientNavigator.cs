using Disk.Entities;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;
using Newtonsoft.Json;

namespace Disk.Navigators
{
    public class EditPatientNavigator : INavigator
    {
        public static void Navigate(INavigationStore navigationStore, Action<Patient>? onCancelEvent, Patient patient)
        {
            navigationStore.SetViewModel<EditPatientViewModel>(
                vm =>
                {
                    vm.IniNavigationStore = navigationStore;
                    vm.Backup = JsonConvert.DeserializeObject<Patient>(JsonConvert.SerializeObject(patient))!;
                    vm.Patient = patient;
                    vm.OnCancelEvent += onCancelEvent;
                });
        }

        public static void NavigateAndClose(INavigationStore navigationStore, Action<Patient>? onCancelEvent, Patient patient)
        {
            if (navigationStore.CanClose)
            {
                navigationStore.Close();
                Navigate(navigationStore, onCancelEvent, patient);
            }
        }

        public static void NavigateWithBar(INavigationStore navigationStore, Action<Patient>? onCancelEvent, Patient patient)
        {
            navigationStore.SetViewModel<NavigationBarLayoutViewModel>(
                vm =>
                {
                    vm.IniNavigationStore = navigationStore;
                    vm.CurrentViewModel = navigationStore.GetViewModel<EditPatientViewModel>(
                    vm =>
                    {
                        vm.IniNavigationStore = navigationStore;
                        vm.Backup = JsonConvert.DeserializeObject<Patient>(JsonConvert.SerializeObject(patient))!;
                        vm.Patient = patient;
                        vm.OnCancelEvent += onCancelEvent;
                    });
                });
        }

        public static void NavigateWithBarAndClose(INavigationStore navigationStore, Action<Patient>? onCancelEvent, Patient patient)
        {
            if (navigationStore.CanClose)
            {
                navigationStore.Close();
                NavigateWithBar(navigationStore, onCancelEvent, patient);
            }
        }
    }
}
