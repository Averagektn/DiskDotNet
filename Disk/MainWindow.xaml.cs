using Disk.Data;
using Disk.Visual;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Disk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<IDrawable> Drawings = [];
        private List<IScalable> Scalings = [];

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}