using System.Windows.Input;

namespace Disk.ViewModel.Common.Commands.Async;

public interface IAsyncCommand : ICommand
{
    Task ExecuteAsync(object parameter);
}
