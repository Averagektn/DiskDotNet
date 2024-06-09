using Disk.Entities;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class AppointmentViewModel(NavigationStore navigationStore) : ObserverViewModel
    {
        public Appointment Appointment { get; set; } = null!;
        public bool IsNewAppointment { get; set; }
        public Patient Patient { get; set; } = AppointmentSession.Patient;
        public Doctor Doctor { get; set; } = AppSession.Doctor;
        public ObservableCollection<Session> Sessions { get; set; } = [];
        public ObservableCollection<PathToTarget> PathsToTargets { get; set; } = [];

        public ICommand StartSessionCommand => new Command(_ => navigationStore.SetViewModel<StartSessionViewModel>());
    }
}