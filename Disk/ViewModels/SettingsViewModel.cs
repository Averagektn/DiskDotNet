using Disk.Calculations.Implementations;
using Disk.Navigators;
using Disk.Stores;
using Disk.ViewModels.Common.Commands.Sync;
using Disk.ViewModels.Common.ViewModels;
using Microsoft.Win32;
using Serilog;
using System.Net;
using System.Windows;
using System.Windows.Input;
using Localization = Disk.Properties.Langs.Settings.SettingsLocalization;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModels;

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

    private int _cursorRadius = Settings.IniCursorRadius;
    public string CursorRadius
    {
        get => _cursorRadius.ToString();
        set
        {
            if (int.TryParse(value, out var res) || res < 1 || res > 15)
            {
                _ = SetProperty(ref _cursorRadius, res);
            }
            else
            {
                Log.Information("Settings: Invalid cursor radius");
                _areValidSettings = false;
                _ = SetProperty(ref _cursorRadius, Settings.IniCursorRadius);
                _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await ShowPopup(header: Localization.InvalidCursorRadius, message: Localization.InvalidCursorRadius);
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
            if (int.TryParse(value, out var res) || _targetRadius < 3 || _targetRadius > 17)
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

        ShotTime = Calculator.RoundToNearest(value: 1000 / Settings.ShotTime, nearest: 5).ToString();

        TargetRadius = Settings.IniTargetRadius.ToString();
        CursorRadius = Settings.IniCursorRadius.ToString();

        TargetTtl = Calculator.RoundToNearest(value: 1000 * Settings.TargetHp / (1000 / Settings.ShotTime), nearest: 100).ToString();
    }

    public override void AfterNavigation()
    {
        base.AfterNavigation();

        var ipChanged = Ip != Settings.IP;
        var cursorPathChanged = CursorFilePath != Settings.CursorFilePath;
        var targetPathChanged = TargetFilePath != Settings.TargetFilePath;
        var shotTimeChanged = ShotTime != Calculator.RoundToNearest(value: 1000 / Settings.ShotTime, nearest: 5).ToString();
        var targetRadiusChanged = TargetRadius != Settings.IniTargetRadius.ToString();
        var cursorRadiusChanged = CursorRadius != Settings.IniCursorRadius.ToString();
        var targetTtlChanged = TargetTtl != Calculator.RoundToNearest(value: 1000 * Settings.TargetHp / (1000 / Settings.ShotTime),
            nearest: 100).ToString();

        if (ipChanged || cursorPathChanged || targetPathChanged || shotTimeChanged || targetRadiusChanged || cursorRadiusChanged
            || targetTtlChanged || targetTtlChanged)
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
        Settings.ShotTime = 1000 / _shotTime;

        Settings.IniCursorRadius = _cursorRadius;
        Settings.IniTargetRadius = _targetRadius;

        // convert ms to int hp
        Settings.TargetHp = _targetTtl * _shotTime / 1000;

        Settings.CursorFilePath = CursorFilePath;
        Settings.TargetFilePath = TargetFilePath;

        Settings.Save();
    }
}
