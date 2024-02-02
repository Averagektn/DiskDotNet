namespace Disk.Data.Interface
{
    /// <summary>
    ///     Represents a logger for logging messages
    /// </summary>
    interface ILogger
    {
        /// <summary>
        ///     Logs the specified data
        /// </summary>
        /// <param name="data">
        ///     The data to be logged
        /// </param>
        void Log(object? data);

        /// <summary>
        ///     Logs the specified data and adds a new line
        /// </summary>
        /// <param name="data">
        ///     The data to be logged
        /// </param>
        void LogLn(object? data);

        /// <summary>
        ///     Asynchronously logs the specified data  
        /// </summary>
        /// <param name="data">
        ///     The data to be logged
        /// </param>
        /// <returns>
        ///     A task representing the asynchronous operation
        /// </returns>
        Task LogAsync(object? data);

        /// <summary>
        ///     Asynchronously logs the specified data and adds a new line
        /// </summary>
        /// <param name="data">
        ///     The data to be logged
        /// </param>
        /// <returns>
        ///     A task representing the asynchronous operation
        /// </returns>
        Task LogLnAsync(object? data);
    }
}
