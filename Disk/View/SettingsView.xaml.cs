﻿using Disk.Visual.Impl;
using Disk.Visual.Interface;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Settings = Disk.Properties.Config.Config;

namespace Disk.View;

public partial class SettingsView : UserControl
{
    private IUser? _user;
    private ITarget? _target;

    private string TargetFilePathText => TargetFilePath.Text;
    private int TargetRadius => (int)TargetRadiusSlider.Value;
    private string UserFilePathText => UserFilePath.Text;
    private int UserRadius => (int)UserRadiusSlider.Value;

    public SettingsView()
    {
        InitializeComponent();

        UserRadiusSlider.ValueChanged += (_, _) => DrawUser();
        UserFilePath.TextChanged += (_, _) => DrawUser();

        TargetRadiusSlider.ValueChanged += (_, _) => DrawTarget();
        TargetFilePath.TextChanged += (_, _) => DrawTarget();

        SizeChanged += (_, _) =>
        {
            DrawUser();
            DrawTarget();
        };
    }

    private void DrawTarget()
    {
        double userColumnWidth = PaintGrid.ColumnDefinitions[0].ActualWidth;
        double targetColumnWidth = PaintGrid.ColumnDefinitions[1].ActualWidth;
        double centerX = (targetColumnWidth / 2) + userColumnWidth;
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

    private void DrawUser()
    {
        double userColumnWidth = PaintGrid.ColumnDefinitions[0].ActualWidth;
        double centerX = userColumnWidth / 2;
        double centerY = PaintGrid.ActualHeight / 2;
        var userCenter = PaintGrid.TransformToAncestor(MainGrid).Transform(new Point(centerX, centerY));
        var screenIniSize = new Size(Settings.Default.IniScreenWidth, Settings.Default.IniScreenHeight);

        _user?.Remove();
        if (File.Exists(UserFilePathText))
        {
            _user = new UserPicture(UserFilePathText, new(0, 0), 0, new(UserRadius * 10, UserRadius * 10), PaintArea, screenIniSize);
        }
        else
        {
            var r = Settings.Default.UserColor.R;
            var g = Settings.Default.UserColor.G;
            var b = Settings.Default.UserColor.B;
            var userBrush = new SolidColorBrush(Color.FromRgb(r, g, b));
            _user = new User(new(0, 0), UserRadius * 5, 0, userBrush, PaintArea, screenIniSize);
        }

        _user.Draw();
        _user.Move(new((int)userCenter.X, (int)userCenter.Y));
    }
}
