namespace Disk.Entity;

public partial class Xray
{
    public long XrId { get; set; }

    public string XrDate { get; set; } = null!;

    public string XrFilePath { get; set; } = null!;

    public string? XrDescription { get; set; }

    public long XrCard { get; set; }

    public virtual Card XrCardNavigation { get; set; } = null!;
}
