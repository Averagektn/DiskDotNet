﻿using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Serilog;
using System.Windows.Input;

namespace Disk.ViewModel;

public class QuestionViewModel : ObserverViewModel
{
    private string _message = string.Empty;
    public required string Message { get => _message; set => SetProperty(ref _message, value); }

    public event Action? BeforeConfirm;
    public event Action? AfterConfirm;
    public event Action? BeforeCancel;
    public event Action? AfterCancel;

    public ICommand ConfirmCommand => new Command(_ =>
    {
        BeforeConfirm?.Invoke();
        IniNavigationStore.Close();
        AfterConfirm?.Invoke();
    });

    public ICommand CancelCommand => new Command(_ =>
    {
        BeforeCancel?.Invoke();
        IniNavigationStore.Close();
        AfterCancel?.Invoke();
    });

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);

        BeforeConfirm = null;
        BeforeCancel = null;
    }
}
