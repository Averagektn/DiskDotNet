using Disk.Data.Interface;
using System.IO;

namespace Disk.Data.Impl
{
    /// <summary>
    /// 
    /// </summary>
    class Logger : ILogger, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly string Filename;

        private static readonly List<Logger> Loggers = [];

        private readonly StreamWriter Writer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">
        /// 
        /// </param>
        private Logger(string filename)
        {
            Filename = filename;
            Writer = new(filename);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Logger GetLogger(string filename)
        {
            var logger = Loggers.FirstOrDefault(s => s.Filename == filename);

            if (logger is null)
            {
                logger = new(filename);

                Loggers.Add(logger);
            }

            return logger;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Loggers.Remove(this);

            Writer.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">
        /// 
        /// </param>
        public void Log(object? data) => Writer.Write(data?.ToString());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public async Task LogAsync(object? data) => await Writer.WriteAsync(data?.ToString());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">
        /// 
        /// </param>
        public void LogLn(object? data) => Writer.WriteLine(data?.ToString());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public async Task LogLnAsync(object? data) => await Writer.WriteLineAsync(data?.ToString());
    }
}
