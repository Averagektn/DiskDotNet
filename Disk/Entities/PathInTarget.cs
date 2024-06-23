namespace Disk.Entities;

public partial class PathInTarget
{
    public long Session { get; set; }

    public long TargetId { get; set; }

    public string CoordinatesJson { get; set; } = null!;

    public float Precision { get; set; }

    public virtual Session SessionNavigation { get; set; } = null!;
}
