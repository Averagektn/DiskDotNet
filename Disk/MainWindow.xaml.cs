using Disk.Visual.Impl;
using System.Windows;
using System.Windows.Media;

namespace Disk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Axis? xAxis;
        private Axis? yAxis;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            xAxis = new Axis(new(0, (int)RenderSize.Height / 2), new((int)RenderSize.Width, (int)RenderSize.Height / 2), 
                RenderSize, Brushes.Black);
            yAxis = new Axis(new((int)RenderSize.Width / 2, 0), new((int)RenderSize.Width / 2, (int)RenderSize.Height), 
                RenderSize, Brushes.Black);

            xAxis.Draw(PaintAreaGrid);
            yAxis.Draw(PaintAreaGrid);
        }

        private void OnSizeChanged(object sender, RoutedEventArgs e)
        {
            xAxis?.Scale(RenderSize);
            yAxis?.Scale(RenderSize);
        }
    }
}