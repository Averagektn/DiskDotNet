using Disk.Entities;
using Disk.Navigators.Interfaces;
using Disk.Stores.Interfaces;
using Disk.ViewModels;
using Disk.ViewModels.Common.ViewModels;

namespace Disk.Navigators;

public class SessionNavigator : INavigator
{
    public static void Navigate(ObserverViewModel currentViewModel, INavigationStore navigationStore, Patient patient,
        Session session)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<SessionViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.Session = session;
            vm.Patient = patient;
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore, Patient patient,
        Session session)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            Navigate(currentViewModel, navigationStore, patient, session);
        }
    }

    public static void NavigateWithBar(ObserverViewModel currentViewModel, INavigationStore navigationStore, Patient patient,
        Session session)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<SessionViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.Session = session;
                vm.Patient = patient;
            });
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateWithBarAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore,
        Patient patient, Session session)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            NavigateWithBar(currentViewModel, navigationStore, patient, session);
        }
    }
}
