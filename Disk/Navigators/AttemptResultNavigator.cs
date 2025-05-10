using Disk.Navigators.Interfaces;
using Disk.Stores.Interfaces;
using Disk.ViewModels;
using Disk.ViewModels.Common.ViewModels;

namespace Disk.Navigators;

public class AttemptResultNavigator : INavigator
{
    public static void Navigate(ObserverViewModel currentViewModel, INavigationStore navigationStore, long attemptId)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<AttemptResultViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.AttemptId = attemptId;
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore, long attemptId)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            Navigate(currentViewModel, navigationStore, attemptId);
        }
    }

    public static void NavigateWithBar(ObserverViewModel currentViewModel, INavigationStore navigationStore, long attemptId)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<AttemptResultViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.AttemptId = attemptId;
            });
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateWithBarAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore,
        long attemptId)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            NavigateWithBar(currentViewModel, navigationStore, attemptId);
        }
    }
}
