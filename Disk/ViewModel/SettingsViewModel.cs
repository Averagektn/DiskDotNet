using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.Win32;
using System.Net;
using System.Windows;
using System.Windows.Input;
using Localization = Disk.Properties.Langs.Settings.SettingsLocalization;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel;

public class SettingsViewModel : ObserverViewModel
{
    private static Settings Settings => Settings.Default;

    private string _ip = Settings.IP;
    public string Ip
    {
        get => _ip;
        set
        {
            if (IPAddress.TryParse(value, out _))
            {
                _ = SetProperty(ref _ip, value);
            }
            else
            {
                _ = MessageBox.Show(Localization.InvalidIpError, Localization.InvalidIpError, MessageBoxButton.OK, MessageBoxImage.Error);
                _ = SetProperty(ref _ip, Settings.IP);
            }
        }
    }

    // Convert ms to hz
    private int _moveTime = RoundToNearest(value: 1000 / Settings.MoveTime, nearest: 5);
    public string MoveTime
    {
        get => _moveTime.ToString();
        set
        {
            if (int.TryParse(value, out var res) || _moveTime >= 1000 || _moveTime <= 1)
            {
                _ = SetProperty(ref _moveTime, res);
            }
            else
            {
                _ = MessageBox.Show(Localization.InvalidMoveTime, "", MessageBoxButton.OK, MessageBoxImage.Error);
                _ = SetProperty(ref _moveTime, Settings.MoveTime);
            }
        }
    }

    // Convert ms to hz
    private int _shotTime = RoundToNearest(value: 1000 / Settings.ShotTime, nearest: 5);
    public string ShotTime
    {
        get => _shotTime.ToString();
        set
        {
            if (int.TryParse(value, out var res) || _shotTime >= 1000 || _shotTime < 1)
            {
                _ = SetProperty(ref _shotTime, res);
            }
            else
            {
                _ = MessageBox.Show(Localization.InvalidShotTime, "", MessageBoxButton.OK, MessageBoxImage.Error);
                _ = SetProperty(ref _shotTime, Settings.ShotTime);
            }
        }
    }

    private int _userRadius = Settings.IniUserRadius;
    public string UserRadius
    {
        get => _userRadius.ToString();
        set
        {
            if (int.TryParse(value, out var res) || res < 1 || res > 15)
            {
                _ = SetProperty(ref _userRadius, res);
            }
            else
            {
                _ = MessageBox.Show(Localization.InvalidUserRadius, "", MessageBoxButton.OK, MessageBoxImage.Error);
                _ = SetProperty(ref _userRadius, Settings.IniUserRadius);
            }
        }
    }

    private int _targetRadius = Settings.IniTargetRadius;
    public string TargetRadius
    {
        get => _targetRadius.ToString();
        set
        {
            if (int.TryParse(value, out var res) || _targetRadius < 1 || _targetRadius > 15)
            {
                _ = SetProperty(ref _targetRadius, res);
            }
            else
            {
                _ = MessageBox.Show(Localization.InvalidTargetRadius, "", MessageBoxButton.OK, MessageBoxImage.Error);
                _ = SetProperty(ref _targetRadius, Settings.IniTargetRadius);
            }
        }
    }

    // convert int hp to ms
    private int _targetTtl = RoundToNearest(value: 1000 * Settings.TargetHp / (1000 / Settings.ShotTime), nearest: 100);
    public string TargetTtl
    {
        get => _targetTtl.ToString();
        set
        {
            if (int.TryParse(value, out var res) || _targetTtl < 1)
            {
                _ = SetProperty(ref _targetTtl, res);
            }
            else
            {
                _ = MessageBox.Show(Localization.InvalidTargetHP, "", MessageBoxButton.OK, MessageBoxImage.Error);
                _ = SetProperty(ref _targetTtl, Settings.TargetHp);
            }
        }
    }

    public ICommand CancelCommand => new Command(_ => IniNavigationStore.Close());
    public ICommand ChangeLanguageCommand => new Command(ChangeLanguage);
    public ICommand SaveCommand => new Command(_ =>
    {
        SaveSettings();
        IniNavigationStore.Close();
    });

    private string _cursorFilePath = Settings.CursorFilePath;
    public string CursorFilePath { get => _cursorFilePath; set => SetProperty(ref _cursorFilePath, value); }

    public ICommand PickImageCommand => new Command(_ =>
    {
        var filePicker = new OpenFileDialog
        {
            Filter = "Images|*.png"
        };

        if (filePicker.ShowDialog() == true)
        {
            CursorFilePath = filePicker.FileName;
        }
    });

    public ICommand ClearImageCommand => new Command(_ =>
    {
        CursorFilePath = string.Empty;
    });

    private void ChangeLanguage(object? parameter)
    {
        if (parameter is not null)
        {
            var selectedLanguage = parameter.ToString();

            if (Settings.Language != selectedLanguage)
            {
                Settings.Language = selectedLanguage;
                Settings.Save();

                RestartApplication();
            }
        }
    }

    private static void RestartApplication()
    {
        var appPath = Environment.ProcessPath;

        if (appPath is not null)
        {
            _ = System.Diagnostics.Process.Start(appPath);
        }

        Application.Current.Shutdown();
    }

    public override void Refresh()
    {
        base.Refresh();

        SaveSettings();
    }

    private void SaveSettings()
    {
        Settings.IP = _ip;

        // Convert hz to ms
        Settings.MoveTime = 1000 / _moveTime;
        Settings.ShotTime = 1000 / _shotTime;

        Settings.IniUserRadius = _userRadius;
        Settings.IniTargetRadius = _targetRadius;

        // convert ms to int hp
        Settings.TargetHp = _targetTtl * _shotTime / 1000;

        Settings.CursorFilePath = CursorFilePath;

        Settings.Save();
    }

    private static int RoundToNearest(int value, int nearest)
    {
        return (int)(Math.Round((double)value / nearest) * nearest);
    }
}
