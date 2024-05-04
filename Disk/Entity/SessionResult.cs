namespace Disk.Entity;

public partial class SessionResult
{
    public long SresId { get; set; }

    public double SresMathExp { get; set; }

    public double SresDeviation { get; set; }

    public double SresDispersion { get; set; }

    public long SresScore { get; set; }

    public virtual Session Sres { get; set; } = null!;
}
