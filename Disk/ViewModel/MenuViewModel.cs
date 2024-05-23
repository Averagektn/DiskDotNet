using Disk.ViewModel.Common;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class MenuViewModel : ObserverViewModel
    {
        // Actions
        public ICommand ChangeLanguage => new Command(ChangeLanguageClick);
        public ICommand MapConstructorClick => new Command(OnMapContructorClick);
        public ICommand SettingsClick => new Command(OnSettingsClick);
        public ICommand StartClick => new Command(OnStartClick);
        public ICommand QuitClick => new Command(OnQuitClick);
        public ICommand CalibrationClick => new Command(OnCalibrationClick);

        private static Settings Settings => Settings.Default;

        public MenuViewModel()
        {
            if (!Directory.Exists(Settings.MAIN_DIR_PATH))
            {
                _ = Directory.CreateDirectory(Settings.MAIN_DIR_PATH);
            }

            if (!Directory.Exists(Settings.MAPS_DIR_PATH))
            {
                _ = Directory.CreateDirectory(Settings.MAPS_DIR_PATH);
            }
        }

        private void ChangeLanguageClick(object? parameter)
        {
            if (parameter is not null)
            {
                var selectedLanguage = parameter.ToString();

                Settings.LANGUAGE = selectedLanguage;
                Settings.Save();

                RestartApplication();
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

        private void OnMapContructorClick(object? parameter)
        {
            Application.Current.MainWindow.Hide();
            _ = new MapCreator().ShowDialog();
            Application.Current.MainWindow.Show();
        }

        private void OnStartClick(object? parameter)
        {
            Application.Current.MainWindow.Hide();
            _ = new UserDataForm().ShowDialog();
            Application.Current.MainWindow.Show();
        }

        private void OnSettingsClick(object? parameter)
        {
            Application.Current.MainWindow.Hide();
            _ = new SettingsWindow().ShowDialog();
            Application.Current.MainWindow.Show();
        }

        private void OnCalibrationClick(object? parameter)
        {
            Application.Current.MainWindow.Hide();
            _ = new CalibrationWindow().ShowDialog();
            Application.Current.MainWindow.Show();
        }

        private void OnQuitClick(object? parameter) => Application.Current.MainWindow.Close();
    }
}