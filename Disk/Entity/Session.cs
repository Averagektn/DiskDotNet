using System;
using System.Collections.Generic;

namespace Disk.Entity;

public partial class Session
{
    public int SesId { get; set; }

    public int SesPatient { get; set; }

    public int SesDoctor { get; set; }

    public int SesMap { get; set; }

    public string SesLogFilePath { get; set; } = null!;

    public string SesDate { get; set; } = null!;

    public virtual ICollection<PathToTarget> PathToTargets { get; set; } = new List<PathToTarget>();

    public virtual Doctor SesDoctorNavigation { get; set; } = null!;

    public virtual Map SesMapNavigation { get; set; } = null!;

    public virtual Patient SesPatientNavigation { get; set; } = null!;

    public virtual SessionResult? SessionResult { get; set; }
}
