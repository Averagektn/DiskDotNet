namespace Disk.Entity;

public partial class Procedure
{
    public long ProId { get; set; }

    public long ProAssignedBy { get; set; }

    public long ProAssignedTo { get; set; }

    public string ProDateTime { get; set; } = null!;

    public string ProName { get; set; } = null!;

    public long? ProCabinet { get; set; }

    public virtual Doctor ProAssignedByNavigation { get; set; } = null!;

    public virtual Patient ProAssignedToNavigation { get; set; } = null!;

    public virtual DoctorCabinet? ProCabinetNavigation { get; set; }
}
