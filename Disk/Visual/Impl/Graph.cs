﻿using Disk.Visual.Interface;
using System.Drawing;
using System.Windows.Markup;

namespace Disk.Visual
{
    class Graph : IDrawable, IScalable
    {
        public Point CurrSize => throw new NotImplementedException();

        public void Draw(IAddChild addChild)
        {
            throw new NotImplementedException();
        }

        public void Scale(Point newSize)
        {
            throw new NotImplementedException();
        }
    }
}
