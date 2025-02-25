using System.Windows.Controls;
using System.Windows.Markup;

namespace Disk.Visual.Interface;

/// <summary>
///     Represents an object that can be drawn
/// </summary>
public interface IDrawable
{
    void Draw();
    void Remove();
}
