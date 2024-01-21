namespace Disk.Data.Interface
{
    /// <summary>
    /// 
    /// </summary>
    interface ILogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">
        /// 
        /// </param>
        void Log(object? data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">
        /// 
        /// </param>
        void LogLn(object? data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        Task LogAsync(object? data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        Task LogLnAsync(object? data);
    }
}
