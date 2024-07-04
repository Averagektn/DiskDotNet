namespace Disk.Entities;

public partial class PathToTarget
{
    public long Session { get; set; }

    public long TargetNum { get; set; }
    public long TargetNumInc => TargetNum + 1;

    public string CoordinatesJson { get; set; } = null!;

    public double AngleDistance { get; set; }

    public double AngleSpeed { get; set; }

    public double ApproachSpeed { get; set; }

    public double Time { get; set; }

    public virtual Session SessionNavigation { get; set; } = null!;
}
