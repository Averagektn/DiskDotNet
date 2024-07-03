using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Windows;
using System.Windows.Input;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class SettingsViewModel(NavigationStore navigationStore) : ObserverViewModel
    {
        private static Settings Settings => Settings.Default;

        private string _ip = Settings.IP;
        public string Ip { get => _ip; set => _ = SetProperty(ref _ip, value); }

        private int _moveTime = Settings.MoveTime;
        public string MoveTime
        {
            get => _moveTime.ToString();
            set
            {
                if (int.TryParse(value, out var res))
                {
                    _ = SetProperty(ref _moveTime, res);
                }
            }
        }

        private int _shotTime = Settings.ShotTime;
        public string ShotTime
        {
            get => _shotTime.ToString();
            set
            {
                if (int.TryParse(value, out var res))
                {
                    _ = SetProperty(ref _shotTime, res);
                }
            }
        }

        private int _userRadius = Settings.IniUserRadius;
        public string UserRadius
        {
            get => _userRadius.ToString();
            set
            {
                if (int.TryParse(value, out var res))
                {
                    _ = SetProperty(ref _userRadius, res);
                }
            }
        }

        private int _targetRadius = Settings.IniTargetRadius;
        public string TargetRadius
        {
            get => _targetRadius.ToString();
            set
            {
                if (int.TryParse(value, out var res))
                {
                    _ = SetProperty(ref _targetRadius, res);
                }
            }
        }

        private int _targetHp = Settings.TargetHp;
        public string TargetHp
        {
            get => _targetHp.ToString();
            set
            {
                if (int.TryParse(value, out var res))
                {
                    _ = SetProperty(ref _targetHp, res);
                }
            }
        }

        public ICommand SaveCommand => new Command(
            _ =>
            {
                Settings.IP = _ip;
                Settings.MoveTime = _moveTime;
                Settings.ShotTime = _shotTime;
                Settings.IniUserRadius = _userRadius;
                Settings.IniTargetRadius = _targetRadius;
                Settings.TargetHp = _targetHp;

                Settings.Save();

                IniNavigationStore.Close();
            });

        public ICommand CancelCommand => new Command(_ => IniNavigationStore.Close());
        public ICommand ChangeLanguageCommand => new Command(ChangeLanguage);

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
    }
}