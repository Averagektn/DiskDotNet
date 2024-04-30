namespace Disk.Entity;

public partial class Contraindication
{
    public long ConId { get; set; }

    public long ConCard { get; set; }

    public string ConName { get; set; } = null!;

    public virtual Card ConCardNavigation { get; set; } = null!;
}
