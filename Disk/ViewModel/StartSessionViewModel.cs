using Disk.Data.Impl;
using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class StartSessionViewModel(NavigationStore navigationStore, ISessionRepository sessionRepository, 
        IMapRepository mapRepository) : ObserverViewModel
    {
        public event Action? OnSessionOver;

        public ObservableCollection<Map> Maps => new(mapRepository.GetAll());
        public Map? SelectedMap { get; set; }
        public ICommand StartSessionCommand => new Command(StartSession);
        private static Settings Settings => Settings.Default;

        private void StartSession(object? obj)
        {
            if (SelectedMap is null)
            {
                return;
            }

            var session = new Session()
            {
                Appointment = AppointmentSession.Appointment.Id,
                DateTime = DateTime.Now.ToString(),
                LogFilePath = $"{Settings.MAIN_DIR_PATH}{Path.DirectorySeparatorChar}" +
                    $"{AppointmentSession.Patient.Surname} {AppointmentSession.Patient.Name}{Path.DirectorySeparatorChar}" +
                    $"{DateTime.Now:dd.MM.yyyy HH-mm-ss}",
                Map = SelectedMap!.Id,
            };
            sessionRepository.Add(session);
            AppointmentSession.CurrentSession = session;

            _ = navigationStore.NavigateBack();
            //_ = navigationStore.NavigateBack();
            // to paint view

            navigationStore.SetViewModel<PaintViewModel>(vm => 
            {
                vm.CurrPath = $"{Settings.MAIN_DIR_PATH}{Path.DirectorySeparatorChar}" +
                    $"{AppointmentSession.Patient.Surname} {AppointmentSession.Patient.Name}{Path.DirectorySeparatorChar}" +
                    $"{DateTime.Now:dd.MM.yyyy HH-mm-ss}";
                vm.TargetCenters = JsonConvert.DeserializeObject<List<Point2D<float>>>(SelectedMap.CoordinatesJson) ?? [];
                vm.OnSessionOver += OnSessionOver;
            });
        }
    }
}