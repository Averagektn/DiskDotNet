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
    public class AppointmentViewModel(NavigationStore navigationStore, ISessionRepository sessionRepository) : ObserverViewModel
    {
        public Appointment Appointment { get; set; } = null!;

        public bool IsNewAppointment { get; set; }
        public Patient Patient { get; set; } = AppointmentSession.Patient;
        public Doctor Doctor { get; set; } = AppSession.Doctor;
        public Session? SelectedSession { get; set; }
        public ObservableCollection<Session> Sessions { get; set; }
            = new(sessionRepository.GetSessionsWithResultsByAppointment(AppointmentSession.Appointment.Id));
        public ObservableCollection<PathToTarget> PathsToTargets { get; set; } = [];

        public ICommand StartSessionCommand => new Command(_ => _navigationStore.SetViewModel<StartSessionViewModel>());
        public ICommand SessionSelectedCommand => new Command(SessionSelected);

        private readonly NavigationStore _navigationStore = navigationStore;

        private void SessionSelected(object? obj)
        {
            PathsToTargets.Clear();

            foreach (var pathToTarget in SelectedSession!.PathToTargets)
            {
                PathsToTargets.Add(pathToTarget);
            }
        }

    }
}