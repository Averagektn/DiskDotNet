namespace Disk.Entity;

public partial class Patient
{
    public long PatId { get; set; }

    public string PatName { get; set; } = null!;

    public string PatSurname { get; set; } = null!;

    public string? PatPatronymic { get; set; }

    public long PatAddress { get; set; }

    public string PatDateOfBirth { get; set; } = null!;

    public string? PatPhoneMobile { get; set; }

    public string? PatPhoneHome { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual Address PatAddressNavigation { get; set; } = null!;

    public virtual ICollection<Procedure> Procedures { get; set; } = new List<Procedure>();
}
