﻿using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Markup;

namespace Disk.Visual.Impl
{
    class Graph : IDrawable, IScalable
    {
        public Size CurrSize { get; protected set; }

        public void Draw(IAddChild addChild)
        {
            throw new NotImplementedException();
        }

        public void Scale(Size newSize)
        {
            throw new NotImplementedException();
        }
    }
}
