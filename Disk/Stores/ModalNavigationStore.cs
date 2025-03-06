using Disk.Stores.Interface;
using Disk.ViewModel.Common.ViewModels;
using Serilog;

namespace Disk.Stores;

public class ModalNavigationStore(Func<Type, ObserverViewModel> getViewModel) : INavigationStore
{
    public static readonly Stack<ObserverViewModel> ViewModels = [];
    public event Action? CurrentViewModelChanged;

    public bool IsOpen => CurrentViewModel != null;
    public bool CanClose => ViewModels.Count > 0;
    public ObserverViewModel? CurrentViewModel => ViewModels.Count == 0 ? null : ViewModels.Peek();

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
        var viewModel = getViewModel.Invoke(typeof(TViewModel));
        parametrizeViewModel((viewModel as TViewModel)!);

        return viewModel;
    }

    public void Close()
    {
        if (CanClose)
        {
            Log.Information($"Closing {ViewModels.Peek().GetType()}");

            ViewModels.Pop().Dispose();
            if (ViewModels.TryPeek(out var modalVm))
            {
                modalVm.Refresh();
            }
            else if (NavigationStore.ViewModels.TryPeek(out var vm))
            {
                vm.Refresh();
            }
            OnCurrentViewModelChanged();
        }
    }

    public void SetViewModel<TViewModel>()
    {
        var viewModel = getViewModel.Invoke(typeof(TViewModel));
        viewModel.Refresh();
        ViewModels.Push(viewModel);

        Log.Information($"Created modal ViewModel {viewModel.GetType()}");

        OnCurrentViewModelChanged();
    }

    public void SetViewModel<TViewModel>(Action<TViewModel> parametrizeViewModel) where TViewModel : class
    {
        var viewModel = getViewModel.Invoke(typeof(TViewModel));
        parametrizeViewModel((viewModel as TViewModel)!);
        viewModel.Refresh();
        ViewModels.Push(viewModel);

        Log.Information($"Created modal ViewModel {viewModel.GetType()}");

        OnCurrentViewModelChanged();
    }

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}
