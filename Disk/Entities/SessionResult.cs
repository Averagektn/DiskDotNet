namespace Disk.Entities;

public partial class SessionResult
{
    public long Id { get; set; }

    public double MathExpY { get; set; }
    public double MathExpX { get; set; }

    public double DeviationX { get; set; }
    public double DeviationY { get; set; }

    public long Score { get; set; }

    public virtual Session Sres { get; set; } = null!;
}
