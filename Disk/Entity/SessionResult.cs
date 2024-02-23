using System;
using System.Collections.Generic;

namespace Disk.Entity;

public partial class SessionResult
{
    public int SresId { get; set; }

    public double SresMathExp { get; set; }

    public double SresDeviation { get; set; }

    public double SresDispersion { get; set; }

    public virtual Session Sres { get; set; } = null!;
}
