using Disk.Navigators.Interfaces;
using Disk.Stores.Interfaces;
using Disk.ViewModels;
using Disk.ViewModels.Common.ViewModels;

namespace Disk.Navigators;

public class QuestionNavigator : INavigator
{
    public static void Navigate(ObserverViewModel currentViewModel, INavigationStore navigationStore, string message,
        Action? beforeConfirm = null,
        Action? beforeCancel = null,
        Action? afterConfirm = null,
        Action? afterCancel = null)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<QuestionViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.Message = message;
            vm.BeforeConfirm += beforeConfirm;
            vm.BeforeCancel += beforeCancel;
            vm.AfterConfirm += afterConfirm;
            vm.AfterCancel += afterCancel;
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore, string message,
        Action? beforeConfirm = null,
        Action? beforeCancel = null,
        Action? afterConfirm = null,
        Action? afterCancel = null)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            Navigate(currentViewModel, navigationStore, message, beforeConfirm, beforeCancel, afterConfirm, afterCancel);
        }
    }

    public static void NavigateWithBar(ObserverViewModel currentViewModel, INavigationStore navigationStore, string message,
        Action? beforeConfirm = null,
        Action? beforeCancel = null,
        Action? afterConfirm = null,
        Action? afterCancel = null)
    {
        currentViewModel.BeforeNavigation();
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<QuestionViewModel>(vm =>
            {
                vm.IniNavigationStore = navigationStore;
                vm.Message = message;
                vm.BeforeConfirm += beforeConfirm;
                vm.BeforeCancel += beforeCancel;
                vm.AfterConfirm += afterConfirm;
                vm.AfterCancel += afterCancel;
            });
        });
        currentViewModel.AfterNavigation();
    }

    public static void NavigateWithBarAndClose(ObserverViewModel currentViewModel, INavigationStore navigationStore,
        string message,
        Action? beforeConfirm = null,
        Action? beforeCancel = null,
        Action? afterConfirm = null,
        Action? afterCancel = null)
    {
        if (currentViewModel.IniNavigationStore.CanClose)
        {
            currentViewModel.IniNavigationStore.Close();
            NavigateWithBar(currentViewModel, navigationStore, message, beforeConfirm, beforeCancel, afterConfirm, afterCancel);
        }
    }
}
