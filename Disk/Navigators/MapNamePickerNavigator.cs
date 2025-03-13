using Disk.Data.Impl;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;
using Disk.ViewModel.Common.ViewModels;

namespace Disk.Navigators;

public class MapNamePickerNavigator : INavigator
{
    public static void Navigate(ObserverViewModel currentViewModel, INavigationStore navigationStore, List<Point2D<float>> map)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<MapNamePickerViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.Map = map;
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore, 
        List<Point2D<float>> map)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            Navigate(currentViewModel, navigationStore, map);
        }
    }

    public static void NavigateWithBar(ObserverViewModel currentViewModel, INavigationStore navigationStore, 
        List<Point2D<float>> map)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<MapNamePickerViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.Map = map;
            });
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateWithBarAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore, 
        List<Point2D<float>> map)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            NavigateWithBar(currentViewModel, navigationStore, map);
        }
    }
}
