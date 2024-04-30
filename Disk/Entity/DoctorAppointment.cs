namespace Disk.Entity;

public partial class DoctorAppointment
{
    public long? DocId { get; set; }

    public string? DocName { get; set; }

    public string? DocSurname { get; set; }

    public string? DocPatronymic { get; set; }

    public long? AppId { get; set; }

    public string? AppTime { get; set; }

    public long? PatId { get; set; }

    public string? PatName { get; set; }

    public string? PatSurname { get; set; }

    public string? PatPatronymic { get; set; }
}
