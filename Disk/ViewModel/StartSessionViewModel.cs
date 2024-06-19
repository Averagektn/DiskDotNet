using Disk.Data.Impl;
using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class StartSessionViewModel(ModalNavigationStore modalNavigationStore, NavigationStore navigationStore,
        ISessionRepository sessionRepository, IMapRepository mapRepository) : ObserverViewModel
    {
        public event Action? OnSessionOver;

        private string _imageFilePath = "Pick a file";
        public string ImageFilePath { get => _imageFilePath; set => SetProperty(ref _imageFilePath, value); }

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
                ImageFilePath = filePicker.FileName;
            }
        }

        private void StartSession(object? obj)
        {
            if (SelectedMap is null)
            {
                return;
            }

            var logPath = $"{Settings.MainDirPath}{Path.DirectorySeparatorChar}" +
                    $"{AppointmentSession.Patient.Surname} {AppointmentSession.Patient.Name}{Path.DirectorySeparatorChar}" +
                    $"{DateTime.Now:dd.MM.yyyy HH-mm-ss}";
            if (!Directory.Exists(logPath))
            {
                _ = Directory.CreateDirectory(logPath);
            }

            var session = new Session()
            {
                Appointment = AppointmentSession.Appointment.Id,
                DateTime = DateTime.Now.ToString(),
                LogFilePath = logPath,
                Map = SelectedMap!.Id,
                MaxXAngle = Settings.XMaxAngle,
                MaxYAngle = Settings.YMaxAngle
            };
            sessionRepository.Add(session);
            AppointmentSession.CurrentSession = session;

            modalNavigationStore.Close();

            navigationStore.SetViewModel<PaintViewModel>(vm =>
            {
                vm.ImagePath = ImageFilePath;
                vm.CurrPath = logPath;
                vm.TargetCenters = JsonConvert.DeserializeObject<List<Point2D<float>>>(SelectedMap.CoordinatesJson) ?? [];
                vm.OnSessionOver += OnSessionOver;
            });
        }
    }
}