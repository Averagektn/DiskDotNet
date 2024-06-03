using System.ComponentModel;
using System.Windows;
using Disk.ViewModel.Common;

namespace Disk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel => (DataContext as MainViewModel)!;

        public MainWindow()
        {
            InitializeComponent();

            Closing += NavigateBack;
        }

        private void NavigateBack(object? obj, CancelEventArgs args)
        {
            if (_viewModel.CanNavigateBack())
            {
                args.Cancel = true;
            }
        }
    }
}
