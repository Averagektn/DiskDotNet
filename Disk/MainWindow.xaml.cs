using Disk.ViewModels.Common.ViewModels;
using Serilog;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Disk;

public partial class MainWindow : Window
{
    private MainViewModel? ViewModel => DataContext as MainViewModel;

    public MainWindow()
    {
        InitializeComponent();

        Closing += OnClosing;
    }

    private void OnClosing(object? obj, CancelEventArgs args)
    {
        Log.Information("App closing");

        if (ViewModel is not null)
        {
            if (ViewModel.IsModalOpen)
            {
                ViewModel.CloseModal();
                args.Cancel = true;
            }
            else
            {
                ViewModel.Dispose();
            }
        }
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Back)
        {
            ViewModel?.Close();
            e.Handled = true;
        }
    }
}
