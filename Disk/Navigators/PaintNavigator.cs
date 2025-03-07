﻿using Disk.Entities;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;

namespace Disk.Navigators;

public class PaintNavigator : INavigator
{
    public static void Navigate(INavigationStore navigationStore, Session session)
    {
        navigationStore.SetViewModel<PaintViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentSession = session;
        });
    }

    public static void NavigateAndClose(INavigationStore navigationStore, Session session)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            Navigate(navigationStore, session);
        }
    }

    public static void NavigateWithBar(INavigationStore navigationStore, Session session)
    {
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<PaintViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.CurrentSession = session;
            });
        });
    }

    public static void NavigateWithBarAndClose(INavigationStore navigationStore, Session session)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            NavigateWithBar(navigationStore, session);
        }
    }
}
