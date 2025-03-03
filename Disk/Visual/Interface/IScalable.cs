namespace Disk.Visual.Interface;

/// <summary>
///     Represents an object that can be scaled
/// </summary>
public interface IScalable
{
    /// <summary>
    ///     Scales all figures. Actual size is received through parent object, which is passed through constructor
    /// </summary>
    void Scale();
}
