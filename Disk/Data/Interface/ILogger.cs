namespace Disk.Data.Interface
{
    interface ILogger
    {
        void Log(object? data);

        void LogLn(object? data);

        Task LogAsync(object? data);

        Task LogLnAsync(object? data);
    }
}
