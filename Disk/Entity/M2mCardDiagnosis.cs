using System;
using System.Collections.Generic;

namespace Disk.Entity;

public partial class M2mCardDiagnosis
{
    public int C2dCard { get; set; }

    public int C2dDiagnosis { get; set; }

    public string C2dDiagnosisStart { get; set; } = null!;

    public string? C2dDiagnosisFinish { get; set; }

    public virtual Card C2dCardNavigation { get; set; } = null!;

    public virtual Diagnosis C2dDiagnosisNavigation { get; set; } = null!;
}
