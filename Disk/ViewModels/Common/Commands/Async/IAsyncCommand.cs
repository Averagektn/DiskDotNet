using System.Windows.Input;

namespace Disk.ViewModels.Common.Commands.Async;

public interface IAsyncCommand : ICommand
{
    Task ExecuteAsync(object parameter);
}
