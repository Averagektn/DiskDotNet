using Disk.Data.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Disk.Visual.Impl
{
    internal class NumberedTarget : Target
    {
        private readonly TextBlock _numberText;

        public NumberedTarget(Point2D<int> center, int radius, Size iniSize, int number) : base(center, radius, iniSize)
        {
            var numSize = number.ToString().Length;

            _numberText = new TextBlock()
            {
                Text = number.ToString(),
                FontSize = (MaxRadius * 2 - Radius * 2) / numSize + Radius * (numSize - 1),
                Foreground = Brushes.DarkBlue
            };
            UpdateMargin();

            for (int i = 1; i < Circles.Count; i++)
            {
                Circles[i] = new Circle(center, radius * (Circles.Count - i), 0, Brushes.White, iniSize);
            }
        }

        public void UpdateNumber(int number) => _numberText.Text = number.ToString();

        public override void Draw(IAddChild addChild)
        {
            base.Draw(addChild);
            addChild.AddChild(_numberText);
        }

        public override void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft)
        {
            base.Move(moveTop, moveRight, moveBottom, moveLeft);
            UpdateMargin();
        }

        public override void Scale(Size newSize)
        {
            base.Scale(newSize);
            var numSize = _numberText.Text.Length;
            _numberText.FontSize = (MaxRadius * 2 - Radius * 2) / numSize + Radius * (numSize - 1);
            UpdateMargin();
        }

        public override void Move(Point2D<int> center)
        {
            base.Move(center);
            UpdateMargin();
        }

        private void UpdateMargin()
        {
            var numSize = _numberText.Text.Length;
            _numberText.Margin = new(Left + MaxRadius / 2, Top - Radius / 2 + MaxRadius * (numSize - 1) / 2, 0, 0);
        }

        public override void Remove(UIElementCollection collection)
        {
            base.Remove(collection);
            if (collection.Contains(_numberText))
            {
                collection.Remove(_numberText);
            }
        }
    }
}
