using Disk.Extensions;

namespace Disk.Entities;

public partial class Doctor
{
    public long Id { get; set; }

    private string _name = null!;
    public string Name { get => _name; set => _name = value.CapitalizeFirstLetter(); }

    private string _surname = null!;
    public string Surname { get => _surname; set => _surname = value.CapitalizeFirstLetter(); }

    private string? _patronymic;
    public string? Patronymic { get => _patronymic; set => _patronymic = value?.CapitalizeFirstLetter(); }

    public string Password { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = [];

    public virtual ICollection<Map> Maps { get; set; } = [];

    public virtual ICollection<Note> Notes { get; set; } = [];

    public override string ToString() => $"{Surname} {Name} {Patronymic}";
}
