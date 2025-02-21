using Disk.Entities;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;

namespace Disk.Navigators;

public class PaintNavigator : INavigator
{
    public static void Navigate(INavigationStore navigationStore, string imageFilePath, string logPath, Action? onSessionOver, 
        Session session)
    {
        navigationStore.SetViewModel<PaintViewModel>(
            vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.ImagePath = imageFilePath;
                vm.CurrentPath = logPath;
                vm.OnSessionOver += onSessionOver;
                vm.CurrentSession = session;
            });
    }

    public static void NavigateAndClose(INavigationStore navigationStore, string imageFilePath, string logPath, Action? onSessionOver,
        Session session)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            Navigate(navigationStore, imageFilePath, logPath, onSessionOver, session);
        }
    }

    public static void NavigateWithBar(INavigationStore navigationStore, string imageFilePath, string logPath, Action? onSessionOver,
        Session session)
    {
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(
            vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.CurrentViewModel = navigationStore.GetViewModel<PaintViewModel>(
                    vm =>
                    {
                        vm.IniNavigationStore = navigationStore;
                        vm.ImagePath = imageFilePath;
                        vm.CurrentPath = logPath;
                        vm.OnSessionOver += onSessionOver;
                        vm.CurrentSession = session;
                    });
            });
    }

    public static void NavigateWithBarAndClose(INavigationStore navigationStore, string imageFilePath, string logPath, 
        Action? onSessionOver, Session session)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            NavigateWithBar(navigationStore, imageFilePath, logPath, onSessionOver, session);
        }
    }
}
