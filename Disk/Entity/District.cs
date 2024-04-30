namespace Disk.Entity;

public partial class District
{
    public long DstId { get; set; }

    public string DstName { get; set; } = null!;

    public long DstRegion { get; set; }

    public virtual Region DstRegionNavigation { get; set; } = null!;
}
