using Disk.Calculations.Implementations.Converters;
using Disk.Data.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Disk.Visual.Implementations;

/// <summary>
///     Used for maps contruction. Contains a number in the center
/// </summary>
public class NumberedTarget : Target
{
    /// <summary>
    ///     Returns x and y coordinates in angles
    /// </summary>
    public Point2D<float> Angles => _converter.ToAngle_FromWnd(Center);

    /// <inheritdoc/>
    public override Point2D<int> Center
    {
        get => base.Center;
        protected set
        {
            base.Center = value;

            _numberTextTranform.X = Left + Radius - (_numberText.ActualWidth / 2);
            _numberTextTranform.Y = Top + Radius - (_numberText.ActualHeight / 2);

            _coordXTranform.X = Right + 2;
            _coordXTranform.Y = Top + Radius - (_coordX.ActualHeight / 2);

            _coordYTranform.X = Left + Radius - (_coordY.ActualWidth / 2);
            _coordYTranform.Y = Top - 2 - _coordY.ActualHeight;
        }
    }
    private readonly TranslateTransform _numberTextTranform = new();
    private readonly TranslateTransform _coordYTranform = new();
    private readonly TranslateTransform _coordXTranform = new();

    private readonly TextBlock _numberText;
    private readonly TextBox _coordY;
    private readonly TextBox _coordX;
    private readonly Converter _converter;

    private float _y;
    private float _x;

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
    ///     Panel, containing all figures
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
    public NumberedTarget(Point2D<int> center, int radius, Panel parent, int number, Size iniSize, Converter converter)
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
            MinWidth = Radius,
        };
        _coordY.LostFocus += (_, _) => OnLostKeyboardFocus(_coordY, ref _y);
        _coordY.RenderTransform = _coordYTranform;

        _coordX = new TextBox()
        {
            Text = $"{_x:f1}",
            MaxLength = 5,
            FontSize = 25,
            MinWidth = Radius,
        };
        _coordX.LostFocus += (_, _) => OnLostKeyboardFocus(_coordX, ref _x);
        _coordX.RenderTransform = _coordXTranform;

        _numberText = new TextBlock()
        {
            Text = number.ToString(),
            Foreground = Brushes.DarkBlue,
        };
        _numberText.SizeChanged += (_, s) =>
        {
            _numberTextTranform.X = Left + Radius - (s.NewSize.Width / 2);
            _numberTextTranform.Y = Top + Radius - (s.NewSize.Height / 2);
        };
        _numberText.RenderTransform = _numberTextTranform;

        Circles = [
            new(center, SingleRadius * 5, 0, Brushes.Red, parent, iniSize),
            new(center, SingleRadius * 4, 0, Brushes.White, parent, iniSize)
        ];
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
        if (IsDrawn)
        {
            return;
        }

        base.Draw();

        _ = Parent.Children.Add(_numberText);
        _ = Parent.Children.Add(_coordY);
        _ = Parent.Children.Add(_coordX);
    }

    /// <inheritdoc/>
    public override void Scale()
    {
        base.Scale();

        _converter.Scale(Parent.RenderSize);
        UpdateNumSize();
    }

    /// <inheritdoc/>
    public override void Move(Point2D<int> center)
    {
        base.Move(center);

        var point = _converter.ToAngle_FromWnd(Center);

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
            _coordXTranform.X = Left - 2 - _coordX.ActualWidth;
        }
        // X coord bottom
        if (Center.Y + (_coordX.ActualHeight / 2) > Parent.ActualHeight)
        {
            _coordXTranform.Y = Top - 2 - _coordX.ActualHeight;
        }
        // X coord top
        if (Center.Y - (_coordX.ActualHeight / 2) < 0)
        {
            _coordXTranform.Y = 2;
        }
        // Y coord left
        if (Center.X - (_coordY.ActualWidth / 2) < 0)
        {
            _coordYTranform.X = 2;
        }
        // Y coord right
        if (Center.X + (_coordY.ActualWidth / 2) > Parent.ActualWidth)
        {
            _coordYTranform.X = Parent.ActualWidth - 2 - _coordY.ActualWidth;
        }
        // Y coord top
        if (Top - _coordY.ActualWidth < 0)
        {
            _coordYTranform.Y = Bottom + 2;
        }
    }

    /// <summary>
    ///     Resize font size
    /// </summary>
    private void UpdateNumSize()
    {
        var numSize = _numberText.Text.Length;
        var fontSize = (double)((Radius * 2) - (SingleRadius * 2)) / numSize;

        if (fontSize < 1)
        {
            _numberText.FontSize = 1;
        }
        else
        {
            _numberText.FontSize = fontSize;
        }
    }

    /// <inheritdoc/>
    public override void Remove()
    {
        if (!IsDrawn)
        {
            return;
        }

        base.Remove();

        Parent.Children.Remove(_numberText);
        Parent.Children.Remove(_coordY);
        Parent.Children.Remove(_coordX);
    }
}
