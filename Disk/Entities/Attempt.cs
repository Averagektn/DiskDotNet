﻿namespace Disk.Entities;

public partial class Attempt
{
    public long Id { get; set; }

    public double MaxXAngle { get; set; }

    public double MaxYAngle { get; set; }

    public int CursorRadius { get; set; }

    public int TargetRadius { get; set; }

    public string LogFilePath { get; set; } = null!;

    public string DateTime { get; set; } = null!;

    public long Session { get; set; }

    public int SamplingInterval { get; set; }

    public virtual ICollection<PathInTarget> PathInTargets { get; set; } = null!;

    public virtual ICollection<PathToTarget> PathToTargets { get; set; } = null!;

    public virtual Session SessionNavigation { get; set; } = null!;

    public virtual AttemptResult? AttemptResult { get; set; }
}
