using System.ComponentModel;
using System.Windows;
using Disk.ViewModel.Common.ViewModels;

namespace Disk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => (DataContext as MainViewModel)!;

        public MainWindow()
        {
            InitializeComponent();

            Closing += OnClosing;
        }

        private void OnClosing(object? obj, CancelEventArgs args)
        {
            ViewModel.Dispose();
        }
    }
}
