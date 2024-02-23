using System;
using System.Collections.Generic;

namespace Disk.Entity;

public partial class PathToTarget
{
    public int PttSession { get; set; }

    public int PttNum { get; set; }

    public string PttCoordinatesJson { get; set; } = null!;

    public virtual Session PttSessionNavigation { get; set; } = null!;
}
