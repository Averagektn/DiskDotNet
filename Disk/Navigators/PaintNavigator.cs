﻿using Disk.Entities;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;

namespace Disk.Navigators;

public class PaintNavigator : INavigator
{
    public static void Navigate(INavigationStore navigationStore, long sessionId)
    {
        navigationStore.SetViewModel<PaintViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.SessionId = sessionId;
        });
    }

    public static void NavigateAndClose(INavigationStore navigationStore, long sessionId)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            Navigate(navigationStore, sessionId);
        }
    }

    public static void NavigateWithBar(INavigationStore navigationStore, long sessionId)
    {
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<PaintViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.SessionId = sessionId;
            });
        });
    }

    public static void NavigateWithBarAndClose(INavigationStore navigationStore, long sessionId)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            NavigateWithBar(navigationStore, sessionId);
        }
    }
}
