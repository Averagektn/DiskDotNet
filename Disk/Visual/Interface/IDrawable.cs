namespace Disk.Visual.Interface;

/// <summary>
///     Represents an object that can be drawn
/// </summary>
public interface IDrawable
{
    /// <summary>
    ///     Draws an object. Parent is passed through constructor
    /// </summary>
    void Draw();

    /// <summary>
    ///     Removes an object. Parent is passed through constructor
    /// </summary>
    void Remove();
}
