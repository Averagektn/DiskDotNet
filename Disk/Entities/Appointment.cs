namespace Disk.Entities;

public partial class Appointment
{
    public long Id { get; set; }

    public string DateTime { get; set; } = null!;

    public long Patient { get; set; }

    public virtual Patient PatientNavigation { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = [];
}
