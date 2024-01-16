using Disk.Data;
using Disk.Data.Impl;
using Disk.Visual;
using Disk.Visual.Interface;
using System.Windows;
using Point3D = Disk.Data.Impl.Point3D;

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