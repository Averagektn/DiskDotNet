namespace Disk.Entity;

public partial class Note
{
    public long NtId { get; set; }

    public long NtPatient { get; set; }

    public long NtDoctor { get; set; }

    public string NtText { get; set; } = null!;

    public virtual Doctor NtDoctorNavigation { get; set; } = null!;

    public virtual Patient NtPatientNavigation { get; set; } = null!;
}
