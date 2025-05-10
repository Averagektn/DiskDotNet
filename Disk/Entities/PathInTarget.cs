namespace Disk.Entities;

public partial class PathInTarget
{
    public long Attempt { get; set; }

    public long TargetId { get; set; }
    public long TargetIdInc => TargetId + 1;

    public string CoordinatesJson { get; set; } = null!;

    public double Accuracy { get; set; }

    public double EllipseArea { get; set; }
    public double ConvexHullArea { get; set; }

    public double MathExpY { get; set; }
    public double MathExpX { get; set; }

    public double DeviationX { get; set; }
    public double DeviationY { get; set; }

    public virtual Attempt AttemptNavigation { get; set; } = null!;
}
