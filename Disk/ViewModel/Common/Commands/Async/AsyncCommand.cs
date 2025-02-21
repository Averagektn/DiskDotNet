namespace Disk.ViewModel.Common.Commands.Async;

public class AsyncCommand(Func<object?, Task> execute, Predicate<object>? canExecute = null) : AsyncCommandBase
{
    public override bool CanExecute(object? parameter) => canExecute is null || (parameter is not null && canExecute(parameter));

    public override Task ExecuteAsync(object? parameter) => execute(parameter);
}
