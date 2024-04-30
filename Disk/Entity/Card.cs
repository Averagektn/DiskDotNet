namespace Disk.Entity;

public partial class Card
{
    public long CrdId { get; set; }

    public long CrdPatient { get; set; }

    public string CrdNumber { get; set; } = null!;

    public virtual ICollection<Contraindication> Contraindications { get; set; } = new List<Contraindication>();

    public virtual Patient CrdPatientNavigation { get; set; } = null!;

    public virtual ICollection<M2mCardDiagnosis> M2mCardDiagnoses { get; set; } = new List<M2mCardDiagnosis>();

    public virtual ICollection<Operation> Operations { get; set; } = new List<Operation>();

    public virtual ICollection<Xray> Xrays { get; set; } = new List<Xray>();
}
