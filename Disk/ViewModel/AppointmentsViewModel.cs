using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.ViewModels;

namespace Disk.ViewModel
{
    public class AppointmentsViewModel(ModalNavigationStore modalNavigationStore, NavigationStore navigationStore,
        IAppointmentRepository appointmentRepository, INoteRepository noteRepository) : ObserverViewModel
    {
        public Patient Patient { get; set; } = AppointmentSession.Patient;
        // patient
        // notes
        // add note
        // appointments
        // to appointment
        // start appointment
    }
}