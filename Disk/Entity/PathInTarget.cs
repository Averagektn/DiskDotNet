namespace Disk.Entity;

public partial class PathInTarget
{
    public int? PitSession { get; set; }

    public int PitTargetId { get; set; }

    public string PitCoordinatesJson { get; set; } = null!;

    public virtual Session? PitSessionNavigation { get; set; }
}
