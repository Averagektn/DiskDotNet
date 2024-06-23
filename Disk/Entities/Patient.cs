using Disk.Extensions;

namespace Disk.Entities;

public partial class Patient
{
    public long Id { get; set; }

    private string _name = null!;
    public string Name { get => _name.CapitalizeFirstLetter(); set => _name = value.CapitalizeFirstLetter(); }

    private string _surname = null!;
    public string Surname { get => _surname.CapitalizeFirstLetter(); set => _surname = value.CapitalizeFirstLetter(); }

    private string? _patronymic;
    public string? Patronymic { get => _patronymic?.CapitalizeFirstLetter(); set => _patronymic = value?.CapitalizeFirstLetter(); }

    public string DateOfBirth { get; set; } = null!;

    public string PhoneMobile { get; set; } = null!;

    public string? PhoneHome { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = [];

    public override string ToString() => $"{Surname} {Name} {Patronymic}";
}
