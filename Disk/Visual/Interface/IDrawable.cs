using System.Windows.Controls;
using System.Windows.Markup;

namespace Disk.Visual.Interface
{
    /// <summary>
    /// 
    /// </summary>
    interface IDrawable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addChild">
        /// 
        /// </param>
        void Draw(IAddChild addChild);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        void Remove(UIElementCollection collection);
    }
}
