using Disk.Navigators.Interface;
using Disk.Stores.Interface;
using Disk.ViewModel;

namespace Disk.Navigators;

public class QuestionNavigator : INavigator
{
    public static void Navigate(INavigationStore navigationStore, string message, Action? onConfirm, Action? onCancel)
    {
        navigationStore.SetViewModel<QuestionViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.Message = message;
            vm.OnConfirm += onConfirm;
            vm.OnCancel += onCancel;
        });
    }

    public static void NavigateAndClose(INavigationStore navigationStore, string message, Action? onConfirm,
        Action? onCancel)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            Navigate(navigationStore, message, onConfirm, onCancel);
        }
    }

    public static void NavigateWithBar(INavigationStore navigationStore, string message, Action? onConfirm,
        Action? onCancel)
    {
        navigationStore.SetViewModel<NavigationBarLayoutViewModel>(vm =>
        {
            vm.IniNavigationStore = navigationStore;
            vm.CurrentViewModel = navigationStore.GetViewModel<QuestionViewModel>(vm =>
            {
                vm.Message = message;
                vm.OnConfirm += onConfirm;
                vm.OnCancel += onCancel;
            });
        });
    }

    public static void NavigateWithBarAndClose(INavigationStore navigationStore, string message, Action? onConfirm,
        Action? onCancel)
    {
        if (navigationStore.CanClose)
        {
            navigationStore.Close();
            NavigateWithBar(navigationStore, message, onConfirm, onCancel);
        }
    }
}
