using Disk.ViewModel.Common.ViewModels;

namespace Disk.Stores.Interface;

public interface INavigationStore
{
    bool CanClose { get; }
    ObserverViewModel? CurrentViewModel { get; }

    ObserverViewModel GetViewModel(Type vmType);
    ObserverViewModel GetViewModel<TViewModel>() where TViewModel : class;
    ObserverViewModel GetViewModel<TViewModel>(Action<TViewModel> parametrizeViewModel) where TViewModel : class;

    void Close();
    void SetViewModel<TViewModel>();
    void SetViewModel<TViewModel>(Action<TViewModel> parametrizeViewModel) where TViewModel : class;
}
