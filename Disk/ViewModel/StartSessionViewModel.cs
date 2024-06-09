using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class StartSessionViewModel(NavigationStore navigationStore, ISessionRepository sessionRepository) : ObserverViewModel
    {
        public ObservableCollection<Map> Maps { get; set; } = [];
        public Map? SelectedMap { get; set; }
        public ICommand StartSessionCommand => new Command(StartSession);

        private void StartSession(object? obj)
        {
            var session = new Session()
            {
                Appointment = AppointmentSession.Appointment.Id,
                DateTime = DateTime.Now.ToString(),
                LogFilePath = "",
                /*LogFilePath =
                $"{Settings.MAIN_DIR_PATH}{Path.DirectorySeparatorChar}" +
                $"{Patient.Surname} {Patient.Name}{Path.DirectorySeparatorChar}" +
                $"{DateTime.Now}",*/
                Map = SelectedMap!.Id,
            };
            sessionRepository.Add(session);
            AppointmentSession.CurrentSession = session;

            _ = navigationStore.NavigateBack();
            _ = navigationStore.NavigateBack();
            // to paint view

            /*            new PaintWindow()
                        {
                            DbTargetFilePath = SelectedTargetFile.Filepath,
                            DbMapCenters = JsonConvert.DeserializeObject<List<Point2D<float>>>(SelectedMap.CoordinatesJson) ?? [],
                            CurrPath =
                $"{Settings.MAIN_DIR_PATH}{Path.DirectorySeparatorChar}" +
                $"{Patient.Surname} {Patient.Name}{Path.DirectorySeparatorChar}" +
                $"{DateTime.Now:dd.MM.yyyy HH-mm-ss}",
                        }
            .ShowDialog();*/
        }
    }
}