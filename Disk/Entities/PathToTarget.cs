namespace Disk.Entities;

public partial class PathToTarget
{
    public long Attempt { get; set; }

    public long TargetNum { get; set; }
    public long TargetNumInc => TargetNum + 1;

    public string CoordinatesJson { get; set; } = null!;

    public double Distance { get; set; }

    public double AverageSpeed { get; set; }
    public double ApproachSpeed { get; set; }

    public double Time { get; set; }

    public double EllipseArea { get; set; }
    public double ConvexHullArea { get; set; }

    public virtual Attempt AttemptNavigation { get; set; } = null!;
}
