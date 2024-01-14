using Disk.Visual.Interface;
using System.Drawing;

namespace Disk.Visual
{
    class Graph : IDrawable, IScalable
    {
        public Point CurrSize => throw new NotImplementedException();

        public void Draw()
        {
            throw new NotImplementedException();
        }

        public void Scale(Point newSize)
        {
            throw new NotImplementedException();
        }
    }
}
