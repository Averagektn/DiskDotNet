using Disk.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Disk.View
{
    /// <summary>
    /// Interaction logic for StartSessionVIew.xaml
    /// </summary>
    public partial class StartSessionView : UserControl
    {
        private StartSessionViewModel? ViewModel => (StartSessionViewModel)DataContext;

        public StartSessionView()
        {
            InitializeComponent();
        }

        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                ViewModel?.FilterMapNames(comboBox.Text);
                comboBox.IsDropDownOpen = true;
                e.Handled = true;
            }
        }
    }
}
