﻿using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Size = System.Windows.Size;

namespace Disk.Visual.Impl
{
    class Circle : IFigure
    {
        public Point2D<int> Center { get; protected set; }

        public int Right => Center.X + Radius;
        public int Top => Center.Y - Radius;
        public int Bottom => Center.Y + Radius;
        public int Left => Center.X - Radius;

        protected int Radius;
        protected int Speed;

        private const float DIAGONAL_CORRECTION = 1.41f;

        private readonly Ellipse Figure;

        private readonly Size IniSize;

        private readonly int IniSpeed;
        private readonly int IniRadius;

        private Size CurrSize { get; set; }

        private bool isDrawn = false;

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

        public bool Contains(Point2D<int> p)
            => Figure.RenderedGeometry.FillContains(p.ToPoint());

        public virtual void Draw(IAddChild addChild)
        {
            if (!isDrawn)
            {
                addChild.AddChild(Figure);
                isDrawn = true;
            }

            Figure.Margin = new(Left, Top, 0, 0);
        }

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
