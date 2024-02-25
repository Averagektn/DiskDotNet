using Disk.Entity;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class UserDataViewModel : INotifyPropertyChanged
    {
        // Properties
        public string UserName { get; set; } = "Тест";
        public string Surname { get; set; } = "Тестов";
        public DateTime? BirthDate { get; set; }
        public int MapId { get; set; }
        public string MapText { get; set; } = string.Empty;
        public IList<string> Maps { get; set; }

        // Actions
        public ICommand StartClick => new Command(OnStartClick);

        public UserDataViewModel()
        {
            Maps = Directory.GetFiles("maps", "*.map", SearchOption.AllDirectories);
        }

        private static Settings Settings => Settings.Default;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (!(Equals(field, newValue)))
            {
                field = (newValue);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

        private void OnStartClick(object? obj)
        {
            if (Surname != string.Empty && UserName != string.Empty)
            {
                Application.Current.MainWindow.Hide();
                new PaintWindow()
                {
                    CurrPath =
                    $"{Settings.MAIN_DIR_PATH}{Path.DirectorySeparatorChar}" +
                    $"{Surname} {UserName}{Path.DirectorySeparatorChar}" +
                    $"{DateTime.Now:dd.MM.yyyy HH-mm-ss}",
                    MapFilePath = MapText
                }
                .ShowDialog();
                Application.Current.MainWindow.Close();
            }
            else
            {
                MessageBox.Show(Properties.Localization.UserData_FieldIsEmpty);
            }
        }
    }
}