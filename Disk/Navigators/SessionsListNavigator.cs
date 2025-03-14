using Disk.Entities;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;
using Disk.ViewModel.Common.ViewModels;

namespace Disk.Navigators;

public class SessionsListNavigator : INavigator
{
    public static void Navigate(ObserverViewModel currentViewModel, INavigationStore navigationStore, Patient patient)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<SessionsListViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.Patient = patient;
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
            vm.CurrentViewModel = navigationStore.GetViewModel<SessionsListViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.Patient = patient;
            });
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateWithBarAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore, 
        Patient patient)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            NavigateWithBar(currentViewModel, navigationStore, patient);
        }
    }
}
