using Disk.Calculations.Impl;
using Disk.Navigators;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.Win32;
using Serilog;
using System.Net;
using System.Windows;
using System.Windows.Input;
using Localization = Disk.Properties.Langs.Settings.SettingsLocalization;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel;

public class SettingsViewModel(ModalNavigationStore modalNavigationStore) : PopupViewModel
{
    private bool _areValidSettings = true;

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
                Log.Information("Settings: Invalid IP");
                _areValidSettings = false;
                _ = SetProperty(ref _ip, Settings.IP);
                _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await ShowPopup(header: Localization.InvalidIpError, message: Localization.InvalidIpError);
                    _areValidSettings = true;
                });
            }
        }
    }

    // Convert ms to hz
    private int _moveTime = Calculator.RoundToNearest(value: 1000 / Settings.MoveTime, nearest: 5);
    public string MoveTime
    {
        get => _moveTime.ToString();
        set
        {
            Log.Information("Settings: Invalid move time");
            if (int.TryParse(value, out var res) || _moveTime >= 1000 || _moveTime <= 1)
            {
                _ = SetProperty(ref _moveTime, res);
            }
            else
            {
                _areValidSettings = false;
                _ = SetProperty(ref _moveTime, Settings.MoveTime);
                _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await ShowPopup(header: Localization.InvalidMoveTime, message: Localization.InvalidMoveTime);
                    _areValidSettings = true;
                });
            }
        }
    }

    // Convert ms to hz
    private int _shotTime = Calculator.RoundToNearest(value: 1000 / Settings.ShotTime, nearest: 5);
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
                Log.Information("Settings: Invalid shot time");
                _areValidSettings = false;
                _ = SetProperty(ref _shotTime, Settings.ShotTime);
                _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await ShowPopup(header: Localization.InvalidShotTime, message: Localization.InvalidShotTime);
                    _areValidSettings = true;
                });
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
                Log.Information("Settings: Invalid user radius");
                _areValidSettings = false;
                _ = SetProperty(ref _userRadius, Settings.IniUserRadius);
                _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await ShowPopup(header: Localization.InvalidUserRadius, message: Localization.InvalidUserRadius);
                    _areValidSettings = true;
                });
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
                Log.Information("Settings: Invalid target radius");
                _areValidSettings = false;
                _ = SetProperty(ref _targetRadius, Settings.IniTargetRadius);
                _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await ShowPopup(header: Localization.InvalidTargetRadius, message: Localization.InvalidTargetRadius);
                    _areValidSettings = true;
                });
            }
        }
    }

    private string _cursorFilePath = Settings.CursorFilePath;
    public string CursorFilePath
    {
        get => _cursorFilePath;
        set
        {
            _ = SetProperty(ref _cursorFilePath, value);
            Log.Information($"Settings: Cursor file path set to {_cursorFilePath}");
        }
    }

    private string _targetFilePath = Settings.TargetFilePath;
    public string TargetFilePath
    {
        get => _targetFilePath;
        set
        {
            _ = SetProperty(ref _targetFilePath, value);
            Log.Information($"Settings: Target file path set to {_targetFilePath}");
        }
    }

    // convert int hp to ms
    private int _targetTtl = Calculator.RoundToNearest(value: 1000 * Settings.TargetHp / (1000 / Settings.ShotTime), nearest: 100);
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
                Log.Information("Settings: Invalid target ttl");
                _areValidSettings = false;
                _ = SetProperty(ref _targetTtl, Settings.TargetHp);
                _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await ShowPopup(header: Localization.InvalidTargetHP, message: Localization.InvalidTargetHP);
                    _areValidSettings = true;
                });
            }
        }
    }

    public ICommand CancelCommand => new Command(_ => IniNavigationStore.Close());

    public static ICommand ChangeLanguageCommand => new Command(parameter =>
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
    });

    public ICommand SaveCommand => new Command(_ =>
    {
        SaveSettings();
        if (_areValidSettings)
        {
            IniNavigationStore.Close();
        }
        else
        {
            _areValidSettings = true;
        }
    });

    public ICommand PickCursorImageCommand => new Command(_ =>
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

    public ICommand ClearCursorImageCommand => new Command(_ =>
    {
        CursorFilePath = string.Empty;
    });

    public ICommand PickTargetImageCommand => new Command(_ =>
    {
        var filePicker = new OpenFileDialog
        {
            Filter = "Images|*.png"
        };

        if (filePicker.ShowDialog() == true)
        {
            TargetFilePath = filePicker.FileName;
        }
    });

    public ICommand ClearTargetImageCommand => new Command(_ =>
    {
        TargetFilePath = string.Empty;
    });

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

        Ip = Settings.IP;

        CursorFilePath = Settings.CursorFilePath;
        TargetFilePath = Settings.TargetFilePath;

        MoveTime = Calculator.RoundToNearest(value: 1000 / Settings.MoveTime, nearest: 5).ToString();
        ShotTime = Calculator.RoundToNearest(value: 1000 / Settings.ShotTime, nearest: 5).ToString();

        TargetRadius = Settings.IniTargetRadius.ToString();
        UserRadius = Settings.IniUserRadius.ToString();

        TargetTtl = Calculator.RoundToNearest(value: 1000 * Settings.TargetHp / (1000 / Settings.ShotTime), nearest: 100).ToString();
    }

    public override void AfterNavigation()
    {
        base.AfterNavigation();

        var ipChanged = Ip != Settings.IP;
        var cursorPathChanged = CursorFilePath != Settings.CursorFilePath;
        var targetPathChanged = TargetFilePath != Settings.TargetFilePath;
        var moveTimeChanged = MoveTime != Calculator.RoundToNearest(value: 1000 / Settings.MoveTime, nearest: 5).ToString();
        var shotTimeChanged = ShotTime != Calculator.RoundToNearest(value: 1000 / Settings.ShotTime, nearest: 5).ToString();
        var targetRadiusChanged = TargetRadius != Settings.IniTargetRadius.ToString();
        var userRadiusChanged = UserRadius != Settings.IniUserRadius.ToString();
        var targetTtlChanged = TargetTtl != Calculator.RoundToNearest(value: 1000 * Settings.TargetHp / (1000 / Settings.ShotTime),
            nearest: 100).ToString();

        if (ipChanged || cursorPathChanged || targetPathChanged || moveTimeChanged || shotTimeChanged || targetRadiusChanged ||
            userRadiusChanged || targetTtlChanged || targetTtlChanged)
        {
            if (modalNavigationStore.CurrentViewModel is not QuestionViewModel)
            {
                QuestionNavigator.Navigate(IniNavigationStore.CurrentViewModel ?? this, modalNavigationStore,
                    message: Localization.UnsavedSettings,
                    beforeConfirm: SaveSettings);
            }
        }
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
        Settings.TargetFilePath = TargetFilePath;

        Settings.Save();
    }
}
