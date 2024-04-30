namespace Disk.Entity;

public partial class TargetFile
{
    public long TfId { get; set; }

    public string TfFilepath { get; set; } = null!;

    public long TfAddedBy { get; set; }

    public virtual Doctor TfAddedByNavigation { get; set; } = null!;
}
