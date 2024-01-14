using System.Drawing;

namespace Disk.Data
{
    interface ILogger
    {
        void Log(Point point);
        void Log(string message);
        void Log(PointF point);
        void LogLn(Point point);
        void LogLn(string message);
        void LogLn(PointF point);
    }
}
