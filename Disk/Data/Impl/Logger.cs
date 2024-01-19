using Disk.Data.Interface;
using System.IO;

namespace Disk.Data.Impl
{
    class Logger : ILogger, IDisposable
    {
        private readonly StreamWriter Writer;
        private static readonly List<Logger> Loggers = [];

        public readonly string Filename;

        private Logger(string filename)
        {
            Filename = filename;
            Writer = new(filename);
        }

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

        public void Dispose()
        {
            Loggers.Remove(this);

            Writer.Close();
        }

        public void Log(object? data)
        {
            Writer.Write(data?.ToString());
        }

        public async Task LogAsync(object? data)
        {
            await Writer.WriteAsync(data?.ToString());
        }

        public void LogLn(object? data)
        {
            Writer.WriteLine(data?.ToString());
        }

        public async Task LogLnAsync(object? data)
        {
            await Writer.WriteLineAsync(data?.ToString());
        }
    }
}
