namespace Disk.Visual.Interface
{
    /// <summary>
    /// 
    /// </summary>
    interface IFigure : IDrawable, IScalable, IMovable
    {
        /// <summary>
        /// 
        /// </summary>
        int Right { get; }

        /// <summary>
        /// 
        /// </summary>
        int Top { get; }

        /// <summary>
        /// 
        /// </summary>
        int Bottom { get; }

        /// <summary>
        /// 
        /// </summary>
        int Left { get; }
    }
}
