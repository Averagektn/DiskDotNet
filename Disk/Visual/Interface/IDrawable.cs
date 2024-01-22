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
    }
}
