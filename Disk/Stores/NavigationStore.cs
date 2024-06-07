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

        public void SetViewModel<TViewModel>()
        {
            ViewModels.Push(getViewModel.Invoke(typeof(TViewModel)));
            OnCurrentViewModelChanged();
        }

        public bool NavigateBack()
        {
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
