namespace Disk.Entities;

public partial class Appointment
{
    public long Id { get; set; }

    public long Map { get; set; }

    public long Patient { get; set; }

    public string Date { get; set; } = null!;

    public virtual Map MapNavigation { get; set; } = null!;

    public virtual Patient PatientNavigation { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = null!;
}
