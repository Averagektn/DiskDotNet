namespace Disk.Entity;

public partial class Address
{
    public int AddrId { get; set; }

    public string AddrRegion { get; set; } = null!;

    public string AddrStreet { get; set; } = null!;

    public int AddrHouse { get; set; }

    public int AddrApartment { get; set; }

    public int AddrCorpus { get; set; }

    public string AddrDistrict { get; set; } = null!;

    public virtual ICollection<Patient> Patients { get; set; } = [];
}
