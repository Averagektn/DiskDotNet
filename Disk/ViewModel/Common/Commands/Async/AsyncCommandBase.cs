using System.Windows.Input;

namespace Disk.ViewModel.Common.Commands.Async
{
    public abstract class AsyncCommandBase : IAsyncCommand
    {
        public abstract bool CanExecute(object? parameter);
        public abstract Task ExecuteAsync(object? parameter);

        public async void Execute(object? parameter)
        {
            await ExecuteAsync(parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        protected static void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
