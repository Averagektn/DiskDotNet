using Disk.ViewModel.Common.ViewModels;

namespace Disk.Stores
{
    public class NavigationStore(Func<Type, ObserverViewModel> getViewModel)
    {
        public readonly Stack<ObserverViewModel> ViewModels = [];
        public event Action? CurrentViewModelChanged;
        
        public ObserverViewModel GetViewModel(Type vmType) => getViewModel.Invoke(vmType);
        public ObserverViewModel GetViewModel<TViewModel>() => getViewModel.Invoke(typeof(TViewModel));
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

        public void SetViewModel<TViewModel>() where TViewModel : class
        {
            ViewModels.Push(getViewModel.Invoke(typeof(TViewModel)));
            OnCurrentViewModelChanged();
        }

        public bool CanNavigateBack => ViewModels.Count > 1;

        public bool NavigateBack()
        {
            if (ViewModels.Count == 0)
            {
                return false;
            }

            ViewModels.Pop().Dispose();

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
