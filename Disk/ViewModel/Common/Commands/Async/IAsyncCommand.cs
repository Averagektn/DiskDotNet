using System.Windows.Input;

namespace Disk.ViewModel.Common
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
