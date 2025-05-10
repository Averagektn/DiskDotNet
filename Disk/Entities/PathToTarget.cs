namespace Disk.Entities;

public partial class PathToTarget
{
    public long Attempt { get; set; }

    public long TargetId { get; set; }
    public long TargetIdInc => TargetId + 1;

    public string CoordinatesJson { get; set; } = null!;

    public double Distance { get; set; }

    public double AverageSpeed { get; set; }
    public double ApproachSpeed { get; set; }

    public double Time { get; set; }

    public virtual Attempt AttemptNavigation { get; set; } = null!;
}
