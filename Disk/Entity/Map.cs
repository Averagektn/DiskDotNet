namespace Disk.Entity;

public partial class Map
{
    public long MapId { get; set; }

    public string MapCoordinatesJson { get; set; } = null!;

    public string MapCreatedAt { get; set; } = null!;

    public long MapCreatedBy { get; set; }

    public string MapName { get; set; } = null!;

    public virtual Doctor MapCreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
