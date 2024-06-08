using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Windows;
using System.Windows.Input;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class MenuViewModel(NavigationStore navigationStore, ModalNavigationStore modalNavigationStore) : ObserverViewModel
    {
        // Actions
        public ICommand ChangeLanguageCommand => new Command(ChangeLanguage);
        public ICommand ToMapConstructorCommand => new Command(ToMapContructor);
        public ICommand ToSettingsCommand => new Command(ToSettings);
        public ICommand ToPatientsCommand => new Command(ToPatients);
        public ICommand QuitCommand => new Command(Quit);
        public ICommand ToCalibrationCommand => new Command(ToCalibration);
        public ICommand LogoutCommand => new Command(Logout);

        private static Settings Settings => Settings.Default;

        private void ChangeLanguage(object? parameter)
        {
            if (parameter is not null)
            {
                var selectedLanguage = parameter.ToString();

                if (Settings.LANGUAGE != selectedLanguage)
                {
                    Settings.LANGUAGE = selectedLanguage;
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

        private void ToMapContructor(object? parameter)
        {
            //Application.Current.MainWindow.Hide();
            //_ = new MapCreator().ShowDialog();
            //Application.Current.MainWindow.Show();
        }

        private void ToPatients(object? parameter)
        {
            //Application.Current.MainWindow.Hide();
            //_ = new UserDataForm().ShowDialog();
            //Application.Current.MainWindow.Show();
        }

        private void ToSettings(object? parameter)
        {
            //Application.Current.MainWindow.Hide();
            //_ = new SettingsWindow().ShowDialog();
            //Application.Current.MainWindow.Show();
        }

        private void ToCalibration(object? parameter)
        {
            //Application.Current.MainWindow.Hide();
            //_ = new CalibrationWindow().ShowDialog();
            //Application.Current.MainWindow.Show();
        }

        private void Quit(object? parameter) => Application.Current.MainWindow.Close();
        private void Logout(object? parameter) => modalNavigationStore.SetViewModel<AuthenticationViewModel>();
    }
}