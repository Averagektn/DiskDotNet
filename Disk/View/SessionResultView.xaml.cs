using Disk.ViewModel;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Controls;

namespace Disk.View
{
    /// <summary>
    /// Interaction logic for SessionResultView.xaml
    /// </summary>
    public partial class SessionResultView : UserControl
    {
        private SessionResultViewModel? ViewModel => DataContext as SessionResultViewModel;

        private Size PaintPanelSize => PaintArea.RenderSize;

        private List<IScalable> Scalables { get; set; } = [];

        public SessionResultView()
        {
            InitializeComponent();

            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel?.Converter.Scale(PaintPanelSize);
            Scalables.ForEach(s => s.Scale(PaintPanelSize));
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PaintArea.Children.Clear();

            if (ViewModel is not null)
            {
                var figures = ViewModel.GetPathAndRose(PaintPanelSize);
                foreach (var figure in figures)
                {
                    Scalables.Add(figure);

                    figure.Scale(PaintPanelSize);
                    figure.Draw(PaintArea);
                }
            }
        }

        private void RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            SelectionChanged(sender, null!);
        }
    }
}
