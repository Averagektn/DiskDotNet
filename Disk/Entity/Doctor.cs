using System;
using System.Collections.Generic;

namespace Disk.Entity;

public partial class Doctor
{
    public int DocId { get; set; }

    public string DocName { get; set; } = null!;

    public string DocSurname { get; set; } = null!;

    public string? DocPatronymic { get; set; }

    public string DocPassword { get; set; } = null!;

    public virtual ICollection<Map> Maps { get; set; } = new List<Map>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
