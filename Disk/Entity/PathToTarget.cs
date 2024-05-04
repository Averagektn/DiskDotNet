namespace Disk.Entity;

public partial class PathToTarget
{
    public long PttSession { get; set; }

    public long PttNum { get; set; }

    public string PttCoordinatesJson { get; set; } = null!;

    public double PttTime { get; set; }

    public double PttAngleDistance { get; set; }

    public double PttAngleSpeed { get; set; }

    public double PttApproachSpeed { get; set; }

    public virtual Session PttSessionNavigation { get; set; } = null!;
}
