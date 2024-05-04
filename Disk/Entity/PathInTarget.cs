namespace Disk.Entity;

public partial class PathInTarget
{
    public long PitSession { get; set; }

    public long PitTargetId { get; set; }

    public string PitCoordinatesJson { get; set; } = null!;

    public virtual Session PitSessionNavigation { get; set; } = null!;
}
