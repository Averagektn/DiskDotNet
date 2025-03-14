namespace Disk.Entities;

public partial class Session
{
    public long Id { get; set; }

    public long Map { get; set; }

    public long Patient { get; set; }

    public string Date { get; set; } = null!;

    public virtual Map MapNavigation { get; set; } = null!;

    public virtual Patient PatientNavigation { get; set; } = null!;

    public virtual ICollection<Attempt> Attempts { get; set; } = null!;
}
