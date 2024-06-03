using Disk.Stores;
using Disk.ViewModel;
using Disk.ViewModel.Common;
using System.Windows;

namespace Disk
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        App()
        {
            Thread.CurrentThread.CurrentUICulture = 
                new System.Globalization.CultureInfo(Disk.Properties.Config.Config.Default.LANGUAGE);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var navigationStore = new NavigationStore();

            navigationStore.CurrentViewModel = new AuthenticationViewModel(navigationStore);

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(navigationStore)
            };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
