namespace Disk.Entity;

public partial class Operation
{
    public long OpId { get; set; }

    public long OpCard { get; set; }

    public long? OpAsingnedBy { get; set; }

    public string OpName { get; set; } = null!;

    public long? OpCabinet { get; set; }

    public string OpDateTime { get; set; } = null!;

    public virtual Doctor? OpAsingnedByNavigation { get; set; }

    public virtual DoctorCabinet? OpCabinetNavigation { get; set; }

    public virtual Card OpCardNavigation { get; set; } = null!;
}
