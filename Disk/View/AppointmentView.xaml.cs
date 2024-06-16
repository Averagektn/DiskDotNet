using System.Windows;
using System.Windows.Controls;

namespace Disk.View
{
    /// <summary>
    /// Interaction logic for AppointmentView.xaml
    /// </summary>
    public partial class AppointmentView : UserControl
    {
        public AppointmentView()
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

            var fontSize = iniFontSize * double.Min(heightScale, widthScale);
            SessionResultGrid.FontSize = fontSize;
            PathToTargetGrid.FontSize = fontSize;
            TbDoctor.FontSize = fontSize;
            TbPatient.FontSize = fontSize;
        }
    }
}
