using Disk.ViewModel.Common.ViewModels;

namespace Disk.Stores
{
    public class NavigationStore(Func<Type, ObserverViewModel> getViewModel)
    {
        public readonly Stack<ObserverViewModel> ViewModels = [];
        public event Action? CurrentViewModelChanged;

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

        public void SetViewModel<TViewModel>() where TViewModel : class
        {
            ViewModels.Push(getViewModel.Invoke(typeof(TViewModel)));
            OnCurrentViewModelChanged();
        }

        public bool NavigateBack()
        {
            if (ViewModels.Count == 0)
            {
                return false;
            }

            _ = ViewModels.Pop();
            
            if (ViewModels.Count == 0)
            {
                return false;
            }
            else
            {
                OnCurrentViewModelChanged();
                return true;
            }
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
