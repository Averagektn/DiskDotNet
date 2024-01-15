using System.Drawing;

namespace Disk.Data
{
    class Logger : ILogger
    {
        private static List<Logger> Loggers = [];
        public readonly char Separator;
        public readonly string Filename;
        private Logger(string filename, char separator)
        {
            Filename = filename;
            Separator = separator;
        }

        public static Logger GetLogger(string filename, char separator)
        {
            var logger = Loggers.FirstOrDefault(s => s.Filename == filename);

            if (logger is null)
            {
                logger = new(filename, separator);

                Loggers.Add(logger);
            }

            return logger;
        }

        public static void Close(string filename)
        {

        }

        public void Log(Point point)
        {
            throw new NotImplementedException();
        }

        public void Log(string message)
        {
            throw new NotImplementedException();
        }

        public void Log(PointF point)
        {
            throw new NotImplementedException();
        }

        public void LogLn(Point point)
        {
            throw new NotImplementedException();
        }

        public void LogLn(string message)
        {
            throw new NotImplementedException();
        }

        public void LogLn(PointF point)
        {
            throw new NotImplementedException();
        }
    }
}
