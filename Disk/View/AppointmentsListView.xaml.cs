using Disk.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Disk.View
{
    /// <summary>
    /// Interaction logic for AppointmentsView.xaml
    /// </summary>
    public partial class AppointmentsListView : UserControl
    {
        public AppointmentsListView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as AppointmentsListViewModel;
            if (vm is not null)
            {
                await vm.LoadData();
            }
        }
    }
}
