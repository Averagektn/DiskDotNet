using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disk.Visual.Interface
{
    internal interface IMovable
    {
        void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft);
    }
}
