using System.IO;
using System.Windows;
using Settings = Disk.Properties.Config.Config;

namespace Disk
{
    /// <summary>
    ///     Interaction logic for UserDataForm.xaml
    /// </summary>
    public partial class UserDataForm : Window
    {
        private static Settings Settings => Settings.Default;

        public UserDataForm()
        {
            InitializeComponent();

            string[] mapFiles = Directory.GetFiles("maps", "*.map", SearchOption.AllDirectories);
            foreach (string mapFile in mapFiles)
            {
                CbMaps.Items.Add(mapFile);
            }
            CbMaps.SelectedIndex = 0;
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            if (TbSurname.Text != string.Empty && TbName.Text != string.Empty)
            {
                Hide();
                new PaintWindow()
                {
                    CurrPath =
                    $"{Settings.MAIN_DIR_PATH}{Path.DirectorySeparatorChar}" +
                    $"{TbSurname.Text} {TbName.Text}{Path.DirectorySeparatorChar}" +
                    $"{DateTime.Now:dd.MM.yyyy HH-mm-ss}",
                    MapFilePath = CbMaps.Text
                }
                .ShowDialog();
                Close();
            }
            else
            {
                MessageBox.Show(Disk.Properties.Localization.UserData_FieldIsEmpty);
            }
        }
    }
}
