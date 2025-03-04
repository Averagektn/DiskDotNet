using Disk.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Disk.View;

public partial class NavigationBarLayoutView : UserControl
{
    private NavigationBarLayoutViewModel? ViewModel => DataContext as NavigationBarLayoutViewModel;

    public NavigationBarLayoutView()
    {
        InitializeComponent();

        Loaded += NavigationBarLayoutView_Loaded;
        Unloaded += NavigationBarLayoutView_Unloaded;
    }

    private void NavigationBarLayoutView_Loaded(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        window.KeyDown += NavigationBarLayoutView_KeyDown;
    }

    private void NavigationBarLayoutView_Unloaded(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        window.KeyDown -= NavigationBarLayoutView_KeyDown;
    }

    private void NavigationBarLayoutView_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Back)
        {
            ViewModel?.NavigateBackCommand.Execute(null);
        }
    }
}
