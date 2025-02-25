using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Disk.Visual.Impl;

public class NumberedTarget : Target
{
    private readonly TextBlock _numberText;
    private readonly TextBox _coordY;
    private readonly TextBox _coordX;
    private readonly Converter _converter;
    private float y;
    private float x;
    public Point2D<float> Angles => new(x, y);

    public NumberedTarget(Point2D<int> center, int radius, Canvas parent, int number, Size iniSize, Converter converter)
        : base(center, radius, parent, iniSize)
    {
        _converter = converter;
        var point = converter.ToAngle_FromWnd(center);
        x = point.X;
        y = point.Y;

        _coordY = new TextBox()
        {
            Text = $"{y:f2}",
            MaxLength = 5,
            FontSize = 15,
        };
        _coordY.TextChanged += (_, _) =>
        {
            if (float.TryParse($"{_coordY.Text:f2}", out var res))
            {
                y = res;
            }
            else
            {
                _coordY.Text = y.ToString();
            }
        };

        _coordX = new TextBox()
        {
            Text = $"{x:f2}",
            MaxLength = 5,
            FontSize = 15,
        };
        _coordX.TextChanged += (_, _) =>
        {
            if (float.TryParse($"{_coordX.Text:f2}", out var res))
            {
                x = res;
            }
            else
            {
                _coordX.Text = x.ToString();
            }
        };

        _numberText = new TextBlock()
        {
            Text = number.ToString(),
            Foreground = Brushes.DarkBlue,
        };
        _numberText.SizeChanged += (_, s) =>
        {
            Canvas.SetLeft(_numberText, Left + MaxRadius - (s.NewSize.Width / 2));
            Canvas.SetTop(_numberText, Top + MaxRadius - (s.NewSize.Height / 2));

            Canvas.SetLeft(_coordY, Left + MaxRadius - (_coordY.ActualWidth / 2));
            Canvas.SetTop(_coordY, Top - 2 - _coordY.ActualHeight);

            Canvas.SetLeft(_coordX, Right + 2);
            Canvas.SetTop(_coordX, Top + MaxRadius - (_coordY.ActualHeight / 2));
        };

        for (int i = 1; i < Circles.Count; i++)
        {
            Circles[i] = new Circle(center, radius * (Circles.Count - i), 0, Brushes.White, parent, iniSize);
        }
    }

    public void HideAngles()
    {
        _coordY.Visibility = Visibility.Hidden;
    }

    public void ShowAngles()
    {
        _coordY.Visibility = Visibility.Visible;
    }

    public void UpdateNumber(int number)
    {
        _numberText.Text = number.ToString();

        UpdateSizes();
    }

    public override void Draw()
    {
        base.Draw();

        _ = Parent.Children.Add(_numberText);
        _ = Parent.Children.Add(_coordY);
        _ = Parent.Children.Add(_coordX);

        ChangePosition();
    }

    public override void Scale()
    {
        base.Scale();

        UpdateSizes();
    }

    public override void Move(Point2D<int> center)
    {
        base.Move(center);
        
        var point = _converter.ToAngle_FromWnd(Center);
        _coordX.Text = $"{point.X:f2}";
        _coordY.Text = $"{point.Y:f2}";

        ChangePosition();
    }

    private void UpdateSizes()
    {
        var numSize = _numberText.Text.Length;
        var fontSize = (double)((MaxRadius * 2) - (Radius * 2)) / numSize;

        _numberText.FontSize = fontSize;
    }

    private void ChangePosition()
    {
        Canvas.SetLeft(_numberText, Left + MaxRadius - (_numberText.ActualWidth / 2));
        Canvas.SetTop(_numberText, Top + MaxRadius - (_numberText.ActualHeight / 2));

        Canvas.SetLeft(_coordY, Left + MaxRadius - (_coordY.ActualWidth / 2));
        Canvas.SetTop(_coordY, Top - 2 - _coordY.ActualHeight);

        Canvas.SetLeft(_coordX, Right + 2);
        Canvas.SetTop(_coordX, Top + MaxRadius - (_coordX.ActualHeight / 2));
    }

    public override void Remove()
    {
        base.Remove();

        Parent.Children.Remove(_numberText);
        Parent.Children.Remove(_coordY);
        Parent.Children.Remove(_coordX);
    }
}
