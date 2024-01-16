using Disk.Visual;
using Disk.Visual.Interface;
using System.Windows;

namespace Disk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<IDrawable> Drawings = [];
        private readonly List<IScalable> Scalings = [];

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}