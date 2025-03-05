using Disk.Stores.Interface;
using Disk.ViewModel.Common.ViewModels;

namespace Disk.Stores;

public class ModalNavigationStore(Func<Type, ObserverViewModel> getViewModel) : INavigationStore
{
    public bool IsOpen => CurrentViewModel != null;
    public bool CanClose => ViewModels.Count > 0;

    public static readonly Stack<ObserverViewModel> ViewModels = [];
    public event Action? CurrentViewModelChanged;

    public ObserverViewModel GetViewModel(Type vmType) => getViewModel.Invoke(vmType);
    public ObserverViewModel GetViewModel<TViewModel>() where TViewModel : class => getViewModel.Invoke(typeof(TViewModel));
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

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }

    public ObserverViewModel? CurrentViewModel => ViewModels.Count == 0 ? null : ViewModels.Peek();

    public void SetViewModel<TViewModel>()
    {
        var vm = getViewModel.Invoke(typeof(TViewModel));
        vm.Refresh();
        ViewModels.Push(vm);
        OnCurrentViewModelChanged();
    }

    public void SetViewModel<TViewModel>(Action<TViewModel> parametrizeViewModel) where TViewModel : class
    {
        var viewModel = getViewModel.Invoke(typeof(TViewModel));
        parametrizeViewModel((viewModel as TViewModel)!);
        viewModel.Refresh();
        ViewModels.Push(viewModel);
        OnCurrentViewModelChanged();
    }
}
