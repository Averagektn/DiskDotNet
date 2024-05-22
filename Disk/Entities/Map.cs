namespace Disk.Entities;

public partial class Map
{
    public long Id { get; set; }

    public string CoordinatesJson { get; set; } = null!;

    public string CreatedAtDateTime { get; set; } = null!;

    public long CreatedBy { get; set; }

    public string Name { get; set; } = null!;

    public virtual Doctor CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = [];
}
