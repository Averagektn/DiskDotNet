using Disk.ViewModel.Common.ViewModels;

namespace Disk.Stores
{
    public class ModalNavigationStore(Func<Type, ObserverViewModel> getViewModel)
    {
        public bool IsOpen => CurrentViewModel != null;

        public void Close()
        {
            ViewModels.Pop();
            OnCurrentViewModelChanged();
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }


        public readonly Stack<ObserverViewModel> ViewModels = [];
        public event Action? CurrentViewModelChanged;

        public ObserverViewModel? CurrentViewModel
        {
            // uncomment for creating new viewModel on back button click
            //get => getViewModel.Invoke(ViewModels.Peek().GetType());
            get => ViewModels.Count == 0 ? null : ViewModels.Peek();
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
    }
}
