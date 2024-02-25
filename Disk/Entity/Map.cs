namespace Disk.Entity;

public partial class Map
{
    public int MapId { get; set; }

    public string MapCoordinatesJson { get; set; } = null!;

    public string MapCreatedAt { get; set; } = null!;

    public int MapCreatedBy { get; set; }

    public virtual Doctor MapCreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = [];
}
