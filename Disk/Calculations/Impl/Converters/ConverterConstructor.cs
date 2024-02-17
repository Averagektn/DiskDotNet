using Disk.Visual.Interface;
using System.Drawing;
using Size = System.Windows.Size;

namespace Disk.Calculations.Impl.Converters
{
    /// <summary>
    ///     Provides conversion between coordinate systems
    /// </summary>
    internal partial class Converter : IScalable
    {
        /// <summary>
        ///     Size of the screen in angle space
        /// </summary>
        private readonly SizeF AngleSize;

        /// <summary>
        ///     Max angle size on both coordinate directions
        /// </summary>
        private readonly SizeF MaxAngle;

        /// <summary>
        ///     Size of the screen
        /// </summary>
        private Size ScreenSize;

        /// <summary>
        ///     Maximum size in logacal coordinates
        /// </summary>
        private Size MaxLogCoord;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConverterWnd"/> class
        /// </summary>
        /// <param name="screenWidth">
        ///     The width of the screen
        /// </param>
        /// <param name="screenHeight">
        ///     The height of the screen
        /// </param>
        /// <param name="angleWidth">
        ///     The width of the angle
        /// </param>
        /// <param name="angleHeight">
        ///     The height of the angle
        /// </param>
        public Converter(int screenWidth, int screenHeight, float angleWidth, float angleHeight)
        {
            ScreenSize = new Size(screenWidth, screenHeight);
            AngleSize = new SizeF(angleWidth, angleHeight);
            MaxAngle = new SizeF(angleWidth / 2, angleHeight / 2);
            MaxLogCoord = new Size(screenWidth / 2, screenHeight / 2);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConverterWnd"/> class
        /// </summary>
        /// <param name="screenSize">
        ///     The size of the screen
        /// </param>
        /// <param name="angleSize">
        ///     The size of the angle
        /// </param>
        public Converter(Point screenSize, PointF angleSize)
        {
            ScreenSize = new Size(ScreenSize.Width, ScreenSize.Height);
            MaxLogCoord = new Size(screenSize.X / 2, screenSize.Y / 2);

            AngleSize = new SizeF(angleSize);
            MaxAngle = new SizeF(angleSize.X / 2, angleSize.Y / 2);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConverterWnd"/> class
        /// </summary>
        /// <param name="screenSize">
        ///     The size of the screen
        /// </param>
        /// <param name="angleSize">
        ///     The size of the angle
        /// </param>
        public Converter(Size screenSize, SizeF angleSize)
        {
            ScreenSize = screenSize;
            MaxLogCoord = new Size(screenSize.Width / 2, screenSize.Height / 2);

            AngleSize = angleSize;
            MaxAngle = new SizeF(angleSize.Width / 2, angleSize.Height / 2);
        }

        /// <summary>
        ///     Scales the converter to a new screen size
        /// </summary>
        /// <param name="newSize">
        ///     The new screen size
        /// </param>
        public void Scale(Size newSize)
        {
            ScreenSize = newSize;
            MaxLogCoord = new(newSize.Width / 2, newSize.Height / 2);
        }
    }
}
