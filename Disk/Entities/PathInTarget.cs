namespace Disk.Entities;

public partial class PathInTarget
{
    public long Attempt { get; set; }

    public long TargetId { get; set; }

    public string CoordinatesJson { get; set; } = null!;

    public double Accuracy { get; set; }

    public double FullPathEllipseArea { get; set; }
    public double FullPathConvexHullArea { get; set; }
    public double EllipseArea { get; set; }
    public double ConvexHullArea { get; set; }

    public virtual Attempt AttemptNavigation { get; set; } = null!;
}
