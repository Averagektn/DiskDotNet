using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;
using Disk.ViewModel.Common.ViewModels;

namespace Disk.Navigators;

public class AddPatientNavigator : INavigator
{
    public static void Navigate(ObserverViewModel currentViewModel, INavigationStore navigationStore)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<AddPatientViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            Navigate(currentViewModel, navigationStore);
        }
    }

    public static void NavigateWithBar(ObserverViewModel currentViewModel, INavigationStore navigationStore)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<AddPatientViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
            });
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateWithBarAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            NavigateWithBar(currentViewModel, navigationStore);
        }
    }
}
