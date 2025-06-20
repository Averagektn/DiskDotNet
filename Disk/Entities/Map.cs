﻿namespace Disk.Entities;

public partial class Map
{
    public long Id { get; set; }

    public string CoordinatesJson { get; set; } = null!;

    public string CreatedAtDateTime { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = null!;

    public override string ToString()
    {
        return Name;
    }
}
