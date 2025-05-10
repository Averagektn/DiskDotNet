namespace Disk.Entities;

public partial class AttemptResult
{
    public long Id { get; set; }

    public long Score { get; set; }

    public string Note { get; set; } = null!;

    public virtual Attempt Attempt { get; set; } = null!;
}
