namespace Disk.Data
{
    class Logger<DataType> : ILogger<DataType>
    {
        private static readonly List<Logger<DataType>> Loggers = [];

        public readonly string Filename;

        private Logger(string filename)
        {
            Filename = filename;
        }

        public static Logger<DataType> GetLogger(string filename)
        {
            var logger = Loggers.FirstOrDefault(s => s.Filename == filename);

            if (logger is null)
            {
                logger = new(filename);

                Loggers.Add(logger);
            }

            return logger;
        }

        public static void Close(string filename)
        {

        }

        public void Log(DataType data)
        {
            var str = data?.ToString();
        }

        public void LogLn(DataType data)
        {
            var str = data?.ToString();
        }
    }
}
