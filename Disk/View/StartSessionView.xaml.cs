using Disk.ViewModel;
using System.Windows.Controls;

namespace Disk.View;

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
