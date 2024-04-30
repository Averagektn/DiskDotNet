namespace Disk.Entity;

public partial class Region
{
    public long RgnId { get; set; }

    public string RgnName { get; set; } = null!;

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
