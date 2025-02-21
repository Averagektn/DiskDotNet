using Disk.Stores.Interface;
using Disk.ViewModel.Common.ViewModels;

namespace Disk.Stores;

public class NavigationStore(Func<Type, ObserverViewModel> getViewModel) : INavigationStore
{
    public readonly Stack<ObserverViewModel> ViewModels = [];
    public event Action? CurrentViewModelChanged;

    public ObserverViewModel GetViewModel(Type vmType) => getViewModel.Invoke(vmType);
    public ObserverViewModel GetViewModel<TViewModel>() where TViewModel : class => getViewModel.Invoke(typeof(TViewModel));
    public ObserverViewModel GetViewModel<TViewModel>(Action<TViewModel> parametrizeViewModel) where TViewModel : class
    {
        var viewModel = getViewModel.Invoke(typeof(TViewModel));
        parametrizeViewModel((viewModel as TViewModel)!);

        return viewModel;
    }


    public ObserverViewModel CurrentViewModel
    {
        // uncomment for creating new viewModel on back button click
        //get => getViewModel.Invoke(ViewModels.Peek().GetType());
        get => ViewModels.Peek();
    }

    public void SetViewModel<TViewModel>(Action<TViewModel> parametrizeViewModel) where TViewModel : class
    {
        var viewModel = getViewModel.Invoke(typeof(TViewModel));
        parametrizeViewModel((viewModel as TViewModel)!);
        ViewModels.Push(viewModel);
        OnCurrentViewModelChanged();
    }

    public void SetViewModel<TViewModel>()
    {
        ViewModels.Push(getViewModel.Invoke(typeof(TViewModel)));
        OnCurrentViewModelChanged();
    }

    public bool CanClose => ViewModels.Count > 1;

    public void Close()
    {
        if (ViewModels.Count != 0)
        {
            ViewModels.Pop().Dispose();
            OnCurrentViewModelChanged();
        }
    }

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}
