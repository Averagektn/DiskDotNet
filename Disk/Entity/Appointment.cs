namespace Disk.Entity;

public partial class Appointment
{
    public long AppId { get; set; }

    public string AppTime { get; set; } = null!;

    public long AppDoctor { get; set; }

    public long AppPatient { get; set; }

    public virtual Doctor AppDoctorNavigation { get; set; } = null!;

    public virtual Patient AppPatientNavigation { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
