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
        public string UserName { get => _userName; set => SetProperty(ref _userName, value); }
        public string Surname { get => _surname; set => SetProperty(ref _surname, value); }
        public DateTime? BirthDate { get => _birthDate; set => SetProperty(ref _birthDate, value); }
        public int MapId { get => _mapId; set => SetProperty(ref _mapId, value); }
        public string MapText { get => _mapText; set => SetProperty(ref _mapText, value); }
        public IReadOnlyList<string> Maps { get => _maps; set => SetProperty(ref _maps, value); }

        private string _userName = "Тест";
        private string _surname = "Тестов";
        private DateTime? _birthDate = null;
        private int _mapId = 0;
        private string _mapText = string.Empty;
        private IReadOnlyList<string> _maps = [];

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
                Application.Current.Windows.OfType<UserDataForm>().First().Hide();
                new PaintWindow()
                {
                    CurrPath =
                    $"{Settings.MAIN_DIR_PATH}{Path.DirectorySeparatorChar}" +
                    $"{Surname} {UserName}{Path.DirectorySeparatorChar}" +
                    $"{DateTime.Now:dd.MM.yyyy HH-mm-ss}",
                    MapFilePath = MapText
                }
                .ShowDialog();
                Application.Current.Windows.OfType<UserDataForm>().First().Close();
            }
            else
            {
                MessageBox.Show(Properties.Localization.UserData_FieldIsEmpty);
            }
        }
    }
}