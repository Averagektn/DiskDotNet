﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disk.Visual
{
    class Enemy : Circle
    {
        public Enemy(Point center, int radius, int speed) : base(center, radius, speed)
        {
        }
    }
}
