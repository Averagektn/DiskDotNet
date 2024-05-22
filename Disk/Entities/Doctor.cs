namespace Disk.Entities;

public partial class Doctor
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = [];

    public virtual ICollection<Map> Maps { get; set; } = [];

    public virtual ICollection<Note> Notes { get; set; } = [];
}
