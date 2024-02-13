﻿using Disk.Data.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XamlRadialProgressBar;

namespace Disk.Visual.Impl
{
    class ProgressTarget : Target
    {
        public ProgressTarget(Point2D<int> center, int radius, Size iniSize) : base(center, radius, iniSize)
        {
            _border = new()
            {
                Margin = new(Left, Top, 0, 0)
            };
        }

        private RadialProgressBar _border;
    }
}
