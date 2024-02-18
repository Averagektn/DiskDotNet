using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Size = System.Windows.Size;

namespace Disk.Visual.Impl
{
    /// <summary>
    ///     Represents a Circle figure that implements the IFigure interface
    /// </summary>
    class Circle : IFigure
    {
        /// <summary>
        ///     Gets or sets the center point of the circle
        /// </summary>
        public Point2D<int> Center { get; protected set; }

        /// <summary>
        ///     Gets the X-coordinate of the right edge of the circle
        /// </summary>
        public int Right => Center.X + Radius;

        /// <summary>
        ///     Gets the Y-coordinate of the top edge of the circle
        /// </summary>
        public int Top => Center.Y - Radius;

        /// <summary>
        ///     Gets the Y-coordinate of the bottom edge of the circle
        /// </summary>
        public int Bottom => Center.Y + Radius;

        /// <summary>
        ///     Gets the X-coordinate of the left edge of the circle
        /// </summary>
        public int Left => Center.X - Radius;

        /// <summary>
        ///     Gets or sets the radius of the circle
        /// </summary>
        public int Radius { get; protected set; }

        /// <summary>
        ///     Gets or sets the speed of the circle
        /// </summary>
        public int Speed { get; protected set; }

        /// <summary>
        ///     Specifies whether figure is drawn or not
        /// </summary>
        protected bool IsDrawn = false;

        /// <summary>
        ///     Correction multiplier for diagonal movement
        /// </summary>
        private const float DIAGONAL_CORRECTION = 1.41f;

        /// <summary>
        ///     Figure to be drawn
        /// </summary>
        private readonly Ellipse Figure;

        /// <summary>
        ///     Initial ize for scaling
        /// </summary>
        private readonly Size IniSize;

        /// <summary>
        ///     Initial speed to be scaled
        /// </summary>
        private readonly int IniSpeed;

        /// <summary>
        ///     Initial radius to be scaled
        /// </summary>
        private readonly int IniRadius;

        /// <summary>
        ///     Gets or sets the current size of the circle
        /// </summary>
        private Size CurrSize { get; set; }

        /// <summary>
        ///     Initializes a new instance of the Circle class with the specified center, radius, speed, color, and initial 
        ///     size
        /// </summary>
        /// <param name="center">
        ///     The center point of the circle
        /// </param>
        /// <param name="radius">
        ///     The radius of the circle
        /// </param>
        /// <param name="speed">
        ///     The speed of the circle
        /// </param>
        /// <param name="color">
        ///     The color of the circle
        /// </param>
        /// <param name="iniSize">
        ///     The initial size of the circle
        /// </param>
        public Circle(Point2D<int> center, int radius, int speed, Brush color, Size iniSize)
        {
            IniRadius = radius;
            IniSpeed = speed;

            Center = center;
            Radius = radius;
            Speed = speed;

            Figure = new()
            {
                Width = radius * 2,
                Height = radius * 2,
                Fill = color,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new(Left, Top, 0, 0)
            };

            CurrSize = iniSize;
            IniSize = iniSize;
        }

        /// <summary>
        ///     Determines whether the circle contains the specified point
        /// </summary>
        /// <param name="p">
        ///     The point to check
        /// </param>
        /// <returns>
        ///     true if the circle contains the point, otherwise false
        /// </returns>
        public bool Contains(Point2D<int> p)
            => Math.Sqrt(Math.Pow((p.X - Center.X) / Radius, 2) + Math.Pow((p.Y - Center.Y) / Radius, 2)) <= 0;

        /// <summary>
        ///     Draws the circle on the specified container
        /// </summary>
        /// <param name="addChild">
        ///     The container to draw the circle on
        /// </param>
        public virtual void Draw(IAddChild addChild)
        {
            if (!IsDrawn)
            {
                addChild.AddChild(Figure);
                Figure.Margin = new(Left, Top, 0, 0);
                IsDrawn = true;
            }
        }

        /// <summary>
        ///     Removes the circle from the specified collection
        /// </summary>
        /// <param name="collection">
        ///     The collection to remove the circle from
        /// </param>
        public virtual void Remove(UIElementCollection collection) => collection.Remove(Figure);

        /// <summary>
        ///     Moves the circle in the specified directions
        /// </summary>
        /// <param name="moveTop">
        ///     Specifies whether to move the circle up
        /// </param>
        /// <param name="moveRight">
        ///     Specifies whether to move the circle to the right
        /// </param>
        /// <param name="moveBottom">
        ///     Specifies whether to move the circle down
        /// </param>
        /// <param name="moveLeft">
        ///     Specifies whether to move the circle to the left
        /// </param>
        public virtual void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft)
        {
            int xSpeed = 0;
            int ySpeed = 0;
            int speed = Speed;

            if ((moveTop || moveBottom) && (moveRight || moveLeft))
            {
                speed = (int)Math.Round(speed / DIAGONAL_CORRECTION);
            }

            if (moveTop)
            {
                ySpeed -= speed;
            }
            if (moveBottom)
            {
                ySpeed += speed;
            }
            if (moveLeft)
            {
                xSpeed -= speed;
            }
            if (moveRight)
            {
                xSpeed += speed;
            }

            if (Left <= 0 && xSpeed < 0)
            {
                xSpeed = 0;
            }
            if (Right >= CurrSize.Width && xSpeed > 0)
            {
                xSpeed = 0;
            }
            if (Top <= 0 && ySpeed < 0)
            {
                ySpeed = 0;
            }
            if (Bottom >= CurrSize.Height && ySpeed > 0)
            {
                ySpeed = 0;
            }

            Center = new(Center.X + xSpeed, Center.Y + ySpeed);

            Figure.Margin = new(Left, Top, 0, 0);
        }

        /// <summary>
        ///     Scales the circle to the specified new size
        /// </summary>
        /// <param name="newSize">
        ///     The new size to scale the circle to
        /// </param>
        public virtual void Scale(Size newSize)
        {
            double coeffX = (double)newSize.Width / IniSize.Width;
            double coeffY = (double)newSize.Height / IniSize.Height;

            Speed = (int)Math.Round(IniSpeed * (coeffX + coeffY) / 2);
            Radius = (int)Math.Round(IniRadius * (coeffX + coeffY) / 2);

            Center = new(
                    (int)Math.Round(Center.X * (newSize.Width / CurrSize.Width)),
                    (int)Math.Round(Center.Y * (newSize.Height / CurrSize.Height))
                );

            Figure.Width = Radius * 2;
            Figure.Height = Radius * 2;

            CurrSize = newSize;

            Figure.Margin = new(Left, Top, 0, 0);
        }

        /// <summary>
        ///     Moves the circle to the specified center point
        /// </summary>
        /// <param name="center">
        ///     The new center point
        /// </param>
        public virtual void Move(Point2D<int> center)
        {
            if (center.X <= CurrSize.Width && center.Y <= CurrSize.Height && center.X > 0 && center.Y > 0)
            {
                Center = center;

                Figure.Margin = new(Left, Top, 0, 0);
            }
        }
    }
}
