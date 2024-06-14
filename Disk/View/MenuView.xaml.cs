using System.Windows;
using System.Windows.Controls;

namespace Disk.View
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();

            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            const int iniFontSize = 15;
            const int iniHeight = 400;
            const int iniWidth = 800;

            double heightScale = e.NewSize.Height / iniHeight;
            double widthScale = e.NewSize.Width / iniWidth;
            Menu.FontSize = iniFontSize * double.Min(widthScale, heightScale);
            Menu.Height = Menu.FontSize + 10;
        }
    }
}
