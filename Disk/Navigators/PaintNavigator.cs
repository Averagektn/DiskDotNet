using Disk.Entities;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;
using Disk.ViewModel.Common.ViewModels;

namespace Disk.Navigators;

public class PaintNavigator : INavigator
{
    public static void Navigate(ObserverViewModel currentViewModel, INavigationStore navigationStore, long sessionId)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<PaintViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.SessionId = sessionId;
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore, long sessionId)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            Navigate(currentViewModel, navigationStore, sessionId);
        }
    }

    public static void NavigateWithBar(ObserverViewModel currentViewModel, INavigationStore navigationStore, long sessionId)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<PaintViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.SessionId = sessionId;
            });
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateWithBarAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore, 
        long sessionId)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            NavigateWithBar(currentViewModel, navigationStore, sessionId);
        }
    }
}
