using Disk.ViewModel.Common.ViewModels;

namespace Disk.Stores
{
    public class ModalNavigationStore(Func<Type, ObserverViewModel> getViewModel)
    {
        public bool IsOpen => CurrentViewModel != null;
        public bool CanClose => CanCloseStack.Count != 0 && CanCloseStack.Peek();

        private readonly Stack<bool> CanCloseStack = [];
        public readonly Stack<ObserverViewModel> ViewModels = [];
        public event Action? CurrentViewModelChanged;

        public void Close()
        {
            CanCloseStack.Pop();
            ViewModels.Pop();
            OnCurrentViewModelChanged();
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }

        public ObserverViewModel? CurrentViewModel
        {
            // uncomment for creating new viewModel on back button click
            //get => getViewModel.Invoke(ViewModels.Peek().GetType());
            get => ViewModels.Count == 0 ? null : ViewModels.Peek();
        }

        public void SetViewModel<TViewModel>(bool canClose = false)
        {
            CanCloseStack.Push(canClose);
            ViewModels.Push(getViewModel.Invoke(typeof(TViewModel)));
            OnCurrentViewModelChanged();
        }
    }
}
