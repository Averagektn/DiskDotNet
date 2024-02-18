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
    }
}
