namespace Disk.Entity;

public partial class Session
{
    public long SesId { get; set; }

    public long SesMap { get; set; }

    public string SesLogFilePath { get; set; } = null!;

    public string SesDate { get; set; } = null!;

    public long SesAppointment { get; set; }

    public virtual ICollection<PathInTarget> PathInTargets { get; set; } = new List<PathInTarget>();

    public virtual ICollection<PathToTarget> PathToTargets { get; set; } = new List<PathToTarget>();

    public virtual Appointment SesAppointmentNavigation { get; set; } = null!;

    public virtual Map SesMapNavigation { get; set; } = null!;

    public virtual SessionResult? SessionResult { get; set; }
}
