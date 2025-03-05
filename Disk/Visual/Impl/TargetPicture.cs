using Disk.Data.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Disk.Visual.Impl;

public class TargetPicture(Point2D<int> center, int radius, int speed, Brush color, Canvas parent, Size iniSize) :
    User(center, radius, speed, color, parent, iniSize)
{
}
