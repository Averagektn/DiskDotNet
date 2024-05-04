namespace Disk.Entity;

public partial class Doctor
{
    public long DocId { get; set; }

    public string DocName { get; set; } = null!;

    public string DocSurname { get; set; } = null!;

    public string? DocPatronymic { get; set; }

    public string DocPassword { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Map> Maps { get; set; } = new List<Map>();

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual ICollection<Operation> Operations { get; set; } = new List<Operation>();

    public virtual ICollection<Procedure> Procedures { get; set; } = new List<Procedure>();

    public virtual ICollection<TargetFile> TargetFiles { get; set; } = new List<TargetFile>();
}
