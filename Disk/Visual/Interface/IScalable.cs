using System.Windows;

namespace Disk.Visual.Interface
{
    /// <summary>
    ///     Represents an object that can be scaled
    /// </summary>
    interface IScalable
    {
        /// <summary>
        ///     Scales the object to the specified size
        /// </summary>
        /// <param name="newSize">
        ///     The new size to scale the object to
        /// </param>
        void Scale(Size newSize);
    }
}
