using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;

namespace Disk.Navigators;

public class QuestionNavigator : INavigator
{
    public static void Navigate(INavigationStore navigationStore, string message, 
        Action? beforeConfirm = null, 
        Action? beforeCancel = null,
        Action? afterConfirm = null,
        Action? afterCancel = null)
    {
        navigationStore.SetViewModel<QuestionViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.Message = message;
            vm.BeforeConfirm += beforeConfirm;
            vm.BeforeCancel += beforeCancel;
            vm.AfterConfirm += afterConfirm;
            vm.AfterCancel += afterCancel;
        });
    }

    public static void NavigateAndClose(INavigationStore navigationStore, string message,
        Action? beforeConfirm = null,
        Action? beforeCancel = null,
        Action? afterConfirm = null,
        Action? afterCancel = null)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            Navigate(navigationStore, message, beforeConfirm, beforeCancel, afterConfirm, afterCancel);
        }
    }

    public static void NavigateWithBar(INavigationStore navigationStore, string message,
        Action? beforeConfirm = null,
        Action? beforeCancel = null,
        Action? afterConfirm = null,
        Action? afterCancel = null)
    {
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<QuestionViewModel>(vm =>
            {
                vm.Message = message;
                vm.BeforeConfirm += beforeConfirm;
                vm.BeforeCancel += beforeCancel;
                vm.AfterConfirm += afterConfirm;
                vm.AfterCancel += afterCancel;
            });
        });
    }

    public static void NavigateWithBarAndClose(INavigationStore navigationStore, string message,
        Action? beforeConfirm = null,
        Action? beforeCancel = null,
        Action? afterConfirm = null,
        Action? afterCancel = null)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            NavigateWithBar(navigationStore, message, beforeConfirm, beforeCancel, afterConfirm, afterCancel);
        }
    }
}
