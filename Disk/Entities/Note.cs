namespace Disk.Entities;

public partial class Note
{
    public long Id { get; set; }

    public long Doctor { get; set; }

    public long Patient { get; set; }

    public string Text { get; set; } = null!;

    public virtual Doctor DoctorNavigation { get; set; } = null!;

    public virtual Patient PatientNavigation { get; set; } = null!;
}
