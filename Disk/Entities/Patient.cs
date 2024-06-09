namespace Disk.Entities;

public partial class Patient
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string DateOfBirth { get; set; } = null!;

    public string PhoneMobile { get; set; } = null!;

    public string? PhoneHome { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = [];

    public virtual ICollection<Note> Notes { get; set; } = [];

    public override string ToString() => $"{Surname} {Name} {Patronymic}";
}
