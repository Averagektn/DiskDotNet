using System;
using System.Collections.Generic;

namespace Disk.Entity;

public partial class Card
{
    public int CrdId { get; set; }

    public int CrdPatient { get; set; }

    public string CrdNumber { get; set; } = null!;

    public virtual Patient CrdPatientNavigation { get; set; } = null!;

    public virtual ICollection<M2mCardDiagnosis> M2mCardDiagnoses { get; set; } = new List<M2mCardDiagnosis>();
}
