using Disk.Stores.Interfaces;
using Disk.ViewModels.Common.ViewModels;

using Serilog;

namespace Disk.Stores;

public class NavigationStore(Func<Type, ObserverViewModel> getViewModel) : INavigationStore
{
    public static readonly Stack<ObserverViewModel> ViewModels = [];
    public event Action? CurrentViewModelChanged;

    public bool CanClose => ViewModels.Count > 1;
    public ObserverViewModel? CurrentViewModel => ViewModels.Count > 0 ? ViewModels.Peek() : null;

    public ObserverViewModel GetViewModel(Type vmType)
    {
        return getViewModel.Invoke(vmType);
    }

    public ObserverViewModel GetViewModel<TViewModel>() where TViewModel : class
    {
        return getViewModel.Invoke(typeof(TViewModel));
    }

    public ObserverViewModel GetViewModel<TViewModel>(Action<TViewModel> parametrizeViewModel) where TViewModel : class
    {
        ObserverViewModel viewModel = getViewModel.Invoke(typeof(TViewModel));
        parametrizeViewModel((viewModel as TViewModel)!);

        return viewModel;
    }

    public void SetViewModel<TViewModel>(Action<TViewModel> parametrizeViewModel) where TViewModel : class
    {
        if (ViewModels.TryPeek(out ObserverViewModel? oldVm))
        {
            oldVm.BeforeNavigation();
        }

        ObserverViewModel viewModel = getViewModel.Invoke(typeof(TViewModel));
        parametrizeViewModel((viewModel as TViewModel)!);
        viewModel.Refresh();
        ViewModels.Push(viewModel);
        Log.Information($"Created ViewModel {viewModel.GetType()}");
        OnCurrentViewModelChanged();

        oldVm?.AfterNavigation();
    }

    public void SetViewModel<TViewModel>()
    {
        if (ViewModels.TryPeek(out ObserverViewModel? oldVm))
        {
            oldVm.BeforeNavigation();
        }

        ObserverViewModel viewModel = getViewModel.Invoke(typeof(TViewModel));
        //viewModel.Refresh();
        ViewModels.Push(viewModel);
        Log.Information($"Created ViewModel {viewModel.GetType()}");
        OnCurrentViewModelChanged();

        oldVm?.AfterNavigation();
    }

    public void Close()
    {
        if (ViewModels.Count > 0)
        {
            ObserverViewModel currVm = ViewModels.Peek();
            Log.Information($"Closing {currVm.GetType()}");

            currVm.BeforeNavigation();
            _ = ViewModels.Pop();
            if (ViewModels.TryPeek(out ObserverViewModel? vm))
            {
                vm.Refresh();
            }

            OnCurrentViewModelChanged();

            currVm.AfterNavigation();
            currVm.Dispose();
        }
    }

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}
