namespace Disk.Entity;

public partial class DoctorCabinet
{
    public long DcId { get; set; }

    public long DcFloor { get; set; }

    public long DcCabinetNum { get; set; }

    public string DcName { get; set; } = null!;

    public virtual ICollection<Operation> Operations { get; set; } = new List<Operation>();

    public virtual ICollection<Procedure> Procedures { get; set; } = new List<Procedure>();
}
