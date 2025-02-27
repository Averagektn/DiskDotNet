﻿namespace Disk.Entities;

public partial class Session
{
    public long Id { get; set; }

    public long Map { get; set; }

    public float MaxXAngle { get; set; }

    public float MaxYAngle { get; set; }

    public string LogFilePath { get; set; } = null!;

    public string DateTime { get; set; } = null!;

    public long Appointment { get; set; }

    public virtual ICollection<PathInTarget> PathInTargets { get; set; } = [];

    public virtual ICollection<PathToTarget> PathToTargets { get; set; } = [];

    public virtual Appointment AppointmentNavigation { get; set; } = null!;

    public virtual Map MapNavigation { get; set; } = null!;

    public virtual SessionResult? SessionResult { get; set; }
}
