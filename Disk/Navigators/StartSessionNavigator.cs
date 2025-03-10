﻿using Disk.Entities;
using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;

namespace Disk.Navigators;

public class StartSessionNavigator : INavigator
{
    public static void Navigate(INavigationStore navigationStore, Appointment appointment, Patient patient)
    {
        navigationStore.SetViewModel<StartSessionViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.Appointment = appointment;
            vm.Patient = patient;
        });
    }

    public static void NavigateAndClose(INavigationStore navigationStore, Appointment appointment, Patient patient)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            Navigate(navigationStore, appointment, patient);
        }
    }

    public static void NavigateWithBar(INavigationStore navigationStore, Appointment appointment, Patient patient)
    {
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<StartSessionViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.Appointment = appointment;
                vm.Patient = patient;
            });
        });
    }

    public static void NavigateWithBarAndClose(INavigationStore navigationStore, Appointment appointment, Patient patient)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            NavigateWithBar(navigationStore, appointment, patient);
        }
    }
}
