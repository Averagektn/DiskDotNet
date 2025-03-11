namespace Disk.Entities;

public partial class Map
{
    public long Id { get; set; }

    public string CoordinatesJson { get; set; } = null!;

    public string CreatedAtDateTime { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = [];

    public override string ToString() => Name;
}
