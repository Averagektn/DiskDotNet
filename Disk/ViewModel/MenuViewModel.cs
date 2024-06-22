using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Windows;
using System.Windows.Input;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class MenuViewModel(NavigationStore navigationStore) : ObserverViewModel
    {
        public ICommand ChangeLanguageCommand => new Command(ChangeLanguage);
        public ICommand ToMapConstructorCommand => new Command(_ => navigationStore.SetViewModel<MapCreatorViewModel>());

        public ICommand ToSettingsCommand => new Command(
            _ => navigationStore.SetViewModel<NavigateBackViewModel>(
                vm => vm.CurrentViewModel = navigationStore.GetViewModel<SettingsViewModel>()
                )
            );
        public ICommand ToPatientsCommand => new Command(
            _ => navigationStore.SetViewModel<NavigateBackViewModel>(
                vm => vm.CurrentViewModel = navigationStore.GetViewModel<PatientsViewModel>()
                )
            );
        public ICommand ToCalibrationCommand =>
            new Command(
                _ => navigationStore.SetViewModel<NavigateBackViewModel>(
                    vm => vm.CurrentViewModel = navigationStore.GetViewModel<CalibrationViewModel>()
                )
            );

        public static ICommand QuitCommand => new Command(_ => Application.Current.MainWindow.Close());

        private static Settings Settings => Settings.Default;

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