using System.Windows.Controls;
using System.Windows.Markup;

namespace Disk.Visual.Interface
{
    /// <summary>
    ///     Represents an object that can be drawn
    /// </summary>
    interface IDrawable
    {
        /// <summary>
        ///     Draws the object and adds it as a child to the specified parent
        /// </summary>
        /// <param name="addChild">
        ///     The parent object to add the drawn object to
        /// </param>
        void Draw(IAddChild addChild);

        /// <summary>
        ///     Removes the object from the specified collection
        /// </summary>
        /// <param name="collection">
        ///     The collection to remove the object from
        /// </param>
        void Remove(UIElementCollection collection);
    }
}
