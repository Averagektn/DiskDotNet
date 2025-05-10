using Disk.Visual.Implementations;
using Disk.Visual.Interfaces;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Settings = Disk.Properties.Config.Config;

namespace Disk.Views;

public partial class SettingsView : UserControl
{
    private ICursor? _cursor;
    private ITarget? _target;

    private string TargetFilePathText => TargetFilePath.Text;
    private int TargetRadius => (int)TargetRadiusSlider.Value;
    private string CursorFilePathText => CursorFilePath.Text;
    private int CursorRadius => (int)CursorRadiusSlider.Value;

    public SettingsView()
    {
        InitializeComponent();

        CursorRadiusSlider.ValueChanged += (_, _) => DrawCursor();
        CursorFilePath.TextChanged += (_, _) => DrawCursor();

        TargetRadiusSlider.ValueChanged += (_, _) => DrawTarget();
        TargetFilePath.TextChanged += (_, _) => DrawTarget();

        SizeChanged += (_, _) =>
        {
            DrawCursor();
            DrawTarget();
        };
    }

    private void DrawTarget()
    {
        double cursorColumnWidth = PaintGrid.ColumnDefinitions[0].ActualWidth;
        double targetColumnWidth = PaintGrid.ColumnDefinitions[1].ActualWidth;
        double centerX = (targetColumnWidth / 2) + cursorColumnWidth;
        double centerY = PaintGrid.ActualHeight / 2;
        var targetCenter = PaintGrid.TransformToAncestor(MainGrid).Transform(new Point(centerX, centerY));
        var screenIniSize = new Size(Settings.Default.IniScreenWidth, Settings.Default.IniScreenHeight);

        _target?.Remove();
        if (File.Exists(TargetFilePathText))
        {
            _target = new TargetPicture
            (
                TargetFilePathText,
                center: new(0, 0),
                imageSize: new(TargetRadius * 10, TargetRadius * 10),
                parent: PaintArea,
                iniSize: screenIniSize,
                hp: 0
            );
        }
        else
        {
            _target = new Target(new(0, 0), TargetRadius * 5, PaintArea, screenIniSize);
        }

        _target.Draw();
        _target.Move(new((int)targetCenter.X, (int)targetCenter.Y));
    }

    private void DrawCursor()
    {
        double cursorColumnWidth = PaintGrid.ColumnDefinitions[0].ActualWidth;
        double centerX = cursorColumnWidth / 2;
        double centerY = PaintGrid.ActualHeight / 2;
        var cursorCenter = PaintGrid.TransformToAncestor(MainGrid).Transform(new Point(centerX, centerY));
        var screenIniSize = new Size(Settings.Default.IniScreenWidth, Settings.Default.IniScreenHeight);

        _cursor?.Remove();
        if (File.Exists(CursorFilePathText))
        {
            _cursor = new CursorPicture(CursorFilePathText, new(0, 0), 0, new(CursorRadius * 10, CursorRadius * 10), PaintArea, screenIniSize);
        }
        else
        {
            var r = Settings.Default.CursorColor.R;
            var g = Settings.Default.CursorColor.G;
            var b = Settings.Default.CursorColor.B;
            var cursorBrush = new SolidColorBrush(Color.FromRgb(r, g, b));
            _cursor = new Cursor(new(0, 0), CursorRadius * 5, 0, cursorBrush, PaintArea, screenIniSize);
        }

        _cursor.Draw();
        _cursor.Move(new((int)cursorCenter.X, (int)cursorCenter.Y));
    }
}
