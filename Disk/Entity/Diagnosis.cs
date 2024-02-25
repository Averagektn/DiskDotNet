namespace Disk.Entity;

public partial class Diagnosis
{
    public int DiaId { get; set; }

    public string DiaName { get; set; } = null!;

    public virtual ICollection<M2mCardDiagnosis> M2mCardDiagnoses { get; set; } = [];
}
