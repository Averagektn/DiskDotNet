using Disk.Stores.Interface;
using Disk.ViewModel.Common.ViewModels;
using Serilog;

namespace Disk.Stores;

public class ModalNavigationStore(Func<Type, ObserverViewModel> getViewModel) : INavigationStore
{
    public static readonly Stack<ObserverViewModel> ViewModels = [];
    public event Action? CurrentViewModelChanged;

    public bool IsOpen => CurrentViewModel is not null;
    public bool CanClose => ViewModels.Count > 0;
    public ObserverViewModel? CurrentViewModel => CanClose ? ViewModels.Peek() : null;

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
        if (ViewModels.Count != 0)
        {
            var currVm = ViewModels.Peek();
            Log.Information($"Closing {currVm.GetType()}");

            currVm.BeforeNavigation();
            ViewModels.Pop().Dispose();
            if (ViewModels.TryPeek(out var vm))
            {
                vm.Refresh();
            }
            currVm.AfterNavigation();

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
