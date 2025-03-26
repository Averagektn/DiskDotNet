namespace Disk.Entities;

public partial class PathInTarget
{
    public long Attempt { get; set; }

    public long TargetId { get; set; }

    public string CoordinatesJson { get; set; } = null!;

    public float Accuracy { get; set; }

    public virtual Attempt AttemptNavigation { get; set; } = null!;
}
