namespace Disk.Entity;

public partial class Address
{
    public long AddrId { get; set; }

    public long AddrRegion { get; set; }

    public string AddrStreet { get; set; } = null!;

    public long AddrHouse { get; set; }

    public long AddrApartment { get; set; }

    public long AddrCorpus { get; set; }

    public virtual Region AddrRegionNavigation { get; set; } = null!;

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
