﻿using System.Windows.Input;

namespace Disk.ViewModels.Common.Commands.Sync;

public class Command(Action<object?> execute, Predicate<object>? canExecute = null) : ICommand
{
    private readonly Predicate<object>? _canExecute = canExecute;
    private readonly Action<object?> _execute = execute;

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute is null || (parameter is not null && _canExecute(parameter));
    }

    public void Execute(object? parameter)
    {
        _execute(parameter);
    }
}
