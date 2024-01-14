using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disk.Data
{
    interface ILogger
    {
        void Log(Point point);
        void Log(string message);
        void Log(PointF point);
        void LogLn(Point point);
        void LogLn(string message);
        void LogLn(Point point);
    }
}
