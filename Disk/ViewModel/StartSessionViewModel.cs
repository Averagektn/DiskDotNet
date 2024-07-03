using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows.Input;
using Localization = Disk.Properties.Langs.StartSession.StartSessionLocalization;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class StartSessionViewModel(ModalNavigationStore modalNavigationStore, NavigationStore navigationStore,
        ISessionRepository sessionRepository, IMapRepository mapRepository) : ObserverViewModel
    {
        public Patient Patient { get; set; } = null!;
        public Appointment Appointment { get; set; } = null!;
        public event Action? OnSessionOver;

        private string _imageFilePath = string.Empty;
        private string _imageFileName = Localization.PickAFile;
        public string ImageFileName { get => _imageFileName; set => SetProperty(ref _imageFileName, value); }

        public ObservableCollection<Map> Maps => new(mapRepository.GetAll());
        public Map? SelectedMap { get; set; }

        public ICommand StartSessionCommand => new Command(StartSession);
        public ICommand PickImageCommand => new Command(PickImage);

        private static Settings Settings => Settings.Default;

        private void PickImage(object? obj)
        {
            var filePicker = new OpenFileDialog
            {
                Filter = "Images|*.png"
            };

            if (filePicker.ShowDialog() == true)
            {
                _imageFilePath = filePicker.FileName;
                ImageFileName = filePicker.SafeFileName;
            }
        }

        private void StartSession(object? obj)
        {
            if (SelectedMap is null)
            {
                return;
            }

            var logPath = $"{Settings.MainDirPath}{Path.DirectorySeparatorChar}" +
                    $"{Patient.Surname} {Patient.Name}{Path.DirectorySeparatorChar}" +
                    $"{DateTime.Now:dd.MM.yyyy HH-mm-ss}";
            if (!Directory.Exists(logPath))
            {
                _ = Directory.CreateDirectory(logPath);
            }

            var session = new Session()
            {
                Appointment = Appointment.Id,
                DateTime = DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                LogFilePath = logPath,
                Map = SelectedMap!.Id,
                MaxXAngle = Settings.XMaxAngle,
                MaxYAngle = Settings.YMaxAngle
            };
            sessionRepository.Add(session);

            modalNavigationStore.Close();

            navigationStore.SetViewModel<PaintViewModel>(vm =>
            {
                vm.ImagePath = _imageFilePath;
                vm.CurrPath = logPath;
                vm.OnSessionOver += OnSessionOver;
                vm.CurrentSession = session;
            });
        }
    }
}