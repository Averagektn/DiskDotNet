using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Serilog;
using System.Windows.Input;

namespace Disk.ViewModel;

public class QuestionViewModel : ObserverViewModel
{
    private string _message = string.Empty;
    public required string Message { get => _message; set => SetProperty(ref _message, value); }

    public event Action? OnConfirm;
    public event Action? OnCancel;

    public ICommand ConfirmCommand => new Command(_ =>
    {
        OnConfirm?.Invoke();
        IniNavigationStore.Close();
    });

    public ICommand CancelCommand => new Command(_ =>
    {
        OnCancel?.Invoke();
        IniNavigationStore.Close();
    });

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);

        OnConfirm = null;
        OnCancel = null;
    }
}
