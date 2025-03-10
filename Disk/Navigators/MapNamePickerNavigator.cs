﻿using Disk.Data.Impl;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;

namespace Disk.Navigators;

public class MapNamePickerNavigator : INavigator
{
    public static void Navigate(INavigationStore navigationStore, List<Point2D<float>> map)
    {
        navigationStore.SetViewModel<MapNamePickerViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.Map = map;
        });
    }

    public static void NavigateAndClose(INavigationStore navigationStore, List<Point2D<float>> map)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            Navigate(navigationStore, map);
        }
    }

    public static void NavigateWithBar(INavigationStore navigationStore, List<Point2D<float>> map)
    {
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<MapNamePickerViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.Map = map;
            });
        });
    }

    public static void NavigateWithBarAndClose(INavigationStore navigationStore, List<Point2D<float>> map)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            NavigateWithBar(navigationStore, map);
        }
    }
}
