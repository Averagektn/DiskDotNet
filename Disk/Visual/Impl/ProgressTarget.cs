using Disk.Data.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using XamlRadialProgressBar;

namespace Disk.Visual.Impl
{
    class ProgressTarget : Target
    {
        public double Progress => _border.Value / _border.Maximum;

        public bool IsFull => (int)_border.Value >= (int)_border.Maximum;

        /// <summary>
        ///     Gets the maximum radius of the target
        /// </summary>
        public new int MaxRadius => Radius * 6;

        /// <summary>
        ///     Gets the X-coordinate of the right edge of the circle
        /// </summary>
        public new int Right => Center.X + MaxRadius;

        /// <summary>
        ///     Gets the Y-coordinate of the top edge of the circle
        /// </summary>
        public new int Top => Center.Y - MaxRadius;

        /// <summary>
        ///     Gets the Y-coordinate of the bottom edge of the circle
        /// </summary>
        public new int Bottom => Center.Y + MaxRadius;

        /// <summary>
        ///     Gets the X-coordinate of the left edge of the circle
        /// </summary>
        public new int Left => Center.X - MaxRadius;

        private readonly RadialProgressBar _border;

        public ProgressTarget(Point2D<int> center, int radius, Size iniSize, int hp) : base(center, radius, iniSize)
        {
            _border = new()
            {
                Margin = new(Left, Top, 0, 0),
                Maximum = hp,
                Foreground = Brushes.Blue,
                Width = radius * 6 * 2,
                Height = radius * 6 * 2,
            };
        }

        public void Reset()
        {
            _border.Value = 0;
        }

        public override int ReceiveShot(Point2D<int> shot)
        {
            var res = base.ReceiveShot(shot);
            if (res != 0)
            {
                _border.Value += res;
            }

            return res;
        }

        public override void Remove(UIElementCollection collection)
        {
            base.Remove(collection);

            if (collection.Contains(_border))
            {
                collection.Remove(_border);
            }
        }

        /// <summary>
        ///     Draws the target by adding each circle to the specified UI element collection
        /// </summary>
        /// <param name="addChild">
        ///     The UI element collection to add the circles to
        /// </param>
        public override void Draw(IAddChild addChild)
        {
            if (!IsDrawn)
            {
                addChild.AddChild(_border);
            }

            base.Draw(addChild);
        }

        /// <summary>
        ///     Moves the target in the specified directions
        /// </summary>
        /// <param name="moveTop">
        ///     Whether to move the target upwards
        /// </param>
        /// <param name="moveRight">
        ///     Whether to move the target to the right
        /// </param>
        /// <param name="moveBottom">
        ///     Whether to move the target downwards
        /// </param>
        /// <param name="moveLeft">
        ///     Whether to move the target to the left
        /// </param>
        public override void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft)
        {
            base.Move(moveTop, moveRight, moveBottom, moveLeft);

            _border.Margin = new(Left, Top, 0, 0);
        }

        public override void Scale(Size newSize)
        {
            base.Scale(newSize);

            _border.Width = MaxRadius * 2;
            _border.Height = MaxRadius * 2;
            _border.Margin = new(Left, Top, 0, 0);
        }

        /// <summary>
        ///     Moves the target to the specified center point
        /// </summary>
        /// <param name="center">
        ///     The new center point of the target
        /// </param>
        public override void Move(Point2D<int> center)
        {
            base.Move(center);
            Center = center;
            _border.Margin = new(Left, Top, 0, 0);
        }
    }
}
