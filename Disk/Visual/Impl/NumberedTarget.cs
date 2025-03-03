using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Disk.Visual.Impl;

/// <summary>
///     Used for maps contruction. Contains a number in the center
/// </summary>
public class NumberedTarget : Target
{
    private readonly TextBlock _numberText;
    private readonly TextBox _coordY;
    private readonly TextBox _coordX;
    private readonly Converter _converter;

    private float _y;
    private float _x;

    /// <summary>
    ///     Returns x and y coordinates in angles
    /// </summary>
    public Point2D<float> Angles => new(_x, _y);

    /// <inheritdoc/>
    public override Point2D<int> Center
    {
        get => base.Center;
        protected set
        {
            base.Center = value;

            Canvas.SetLeft(_numberText, Left + MaxRadius - (_numberText.ActualWidth / 2));
            Canvas.SetTop(_numberText, Top + MaxRadius - (_numberText.ActualHeight / 2));

            Canvas.SetLeft(_coordY, Left + MaxRadius - (_coordY.ActualWidth / 2));
            Canvas.SetTop(_coordY, Top - 2 - _coordY.ActualHeight);

            Canvas.SetLeft(_coordX, Right + 2);
            Canvas.SetTop(_coordX, Top + MaxRadius - (_coordX.ActualHeight / 2));
        }
    }

    /// <summary>
    ///     Represents a target with a center point, radius, and initial size. Uses numbers to show order
    /// </summary>
    /// <param name="center">
    ///     The center point of the target
    /// </param>
    /// <param name="radius">
    ///     The radius of the target
    /// </param>
    /// <param name="parent">
    ///     Canvas, containing all figures
    /// </param>
    /// <param name="number">
    ///     Initial number to be drawn
    /// </param>
    /// <param name="iniSize">
    ///     The initial size of the target
    /// </param>
    /// <param name="converter">
    ///     Conversion between window and angle coordinates
    /// </param>
    public NumberedTarget(Point2D<int> center, int radius, Canvas parent, int number, Size iniSize, Converter converter)
        : base(center, radius, parent, iniSize)
    {
        _converter = converter;
        var point = converter.ToAngle_FromWnd(center);
        _x = point.X;
        _y = point.Y;

        void OnLostKeyboardFocus(TextBox textBox, ref float coord)
        {
            if (float.TryParse($"{textBox.Text:f1}", out var res))
            {
                coord = res;
                var newCenter = converter.ToWndCoord(new Point2D<float>(_x, _y));
                Move(newCenter);
            }
            else
            {
                textBox.Text = $"{coord:f1}";
            }
        }

        _coordY = new TextBox()
        {
            Text = $"{_y:f1}",
            MaxLength = 5,
            FontSize = 25,
            MinWidth = MaxRadius,
        };
        _coordY.LostFocus += (_, _) => OnLostKeyboardFocus(_coordY, ref _y);

        _coordX = new TextBox()
        {
            Text = $"{_x:f1}",
            MaxLength = 5,
            FontSize = 25,
            MinWidth = MaxRadius,
        };
        _coordX.LostFocus += (_, _) => OnLostKeyboardFocus(_coordX, ref _x);

        _numberText = new TextBlock()
        {
            Text = number.ToString(),
            Foreground = Brushes.DarkBlue,
        };
        _numberText.SizeChanged += (_, s) =>
        {
            Canvas.SetLeft(_numberText, Left + MaxRadius - (s.NewSize.Width / 2));
            Canvas.SetTop(_numberText, Top + MaxRadius - (s.NewSize.Height / 2));
        };

        for (int i = 1; i < Circles.Count; i++)
        {
            Circles[i] = new Circle(center, radius * (Circles.Count - i), 0, Brushes.White, parent, iniSize);
        }
    }

    /// <summary>
    ///     Hides text boxes with angles
    /// </summary>
    public void HideAngles()
    {
        _coordY.Visibility = Visibility.Hidden;
        _coordX.Visibility = Visibility.Hidden;
    }

    /// <summary>
    ///     Shows text boxes with angles
    /// </summary>
    public void ShowAngles()
    {
        _coordY.Visibility = Visibility.Visible;
        _coordX.Visibility = Visibility.Visible;
    }

    /// <summary>
    ///     Set new number
    /// </summary>
    /// <param name="number">
    ///     New number
    /// </param>
    public void UpdateNumber(int number)
    {
        _numberText.Text = number.ToString();
        UpdateNumSize();
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        base.Draw();

        _ = Parent.Children.Add(_numberText);
        _ = Parent.Children.Add(_coordY);
        _ = Parent.Children.Add(_coordX);
    }

    /// <inheritdoc/>
    public override void Scale()
    {
        base.Scale();

        UpdateNumSize();
    }

    /// <inheritdoc/>
    public override void Move(Point2D<int> center)
    {
        base.Move(center);

        var point = _converter.ToAngle_FromWnd(center);
        _coordX.Text = $"{point.X:f1}";
        _coordY.Text = $"{point.Y:f1}";

        CheckOutOfBounds();
    }

    /// <summary>
    ///     If <see cref="_coordX"/> or <see cref="_coordY"/> are out of bounds, it will be moved 
    /// </summary>
    private void CheckOutOfBounds()
    {
        // X coord right
        if (Right + _coordX.ActualWidth > Parent.ActualWidth)
        {
            Canvas.SetLeft(_coordX, Left - 2 - _coordX.ActualWidth);
        }
        // X coord bottom
        if (Center.Y + (_coordX.ActualHeight / 2) > Parent.ActualHeight)
        {
            Canvas.SetTop(_coordX, Parent.ActualHeight - 2 - _coordX.ActualHeight);
        }
        // X coord top
        if (Center.Y - (_coordX.ActualHeight / 2) < 0)
        {
            Canvas.SetTop(_coordX, 2);
        }
        // Y coord left
        if (Center.X - (_coordY.ActualWidth / 2) < 0)
        {
            Canvas.SetLeft(_coordY, 2);
        }
        // Y coord right
        if (Center.X + (_coordY.ActualWidth / 2) > Parent.ActualWidth)
        {
            Canvas.SetLeft(_coordY, Parent.ActualWidth - 2 - _coordY.ActualWidth);
        }
        // Y coord top
        if (Top - _coordY.ActualWidth < 0)
        {
            Canvas.SetTop(_coordY, Bottom + 2);
        }
    }

    /// <summary>
    ///     Resize font size
    /// </summary>
    private void UpdateNumSize()
    {
        var numSize = _numberText.Text.Length;
        var fontSize = (double)((MaxRadius * 2) - (Radius * 2)) / numSize;
        _numberText.FontSize = fontSize;
    }

    /// <inheritdoc/>
    public override void Remove()
    {
        base.Remove();

        Parent.Children.Remove(_numberText);
        Parent.Children.Remove(_coordY);
        Parent.Children.Remove(_coordX);
    }
}
