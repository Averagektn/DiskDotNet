using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class AppointmentsListViewModel(ModalNavigationStore modalNavigationStore, NavigationStore navigationStore,
            IAppointmentRepository appointmentRepository, INoteRepository noteRepository) : ObserverViewModel
    {
        public Patient Patient { get; set; } = null!;
        public ObservableCollection<Note> Notes { get; set; } = null!;
        public ObservableCollection<Appointment> Appointments { get; set; } = null!;

        public Appointment? SelectedAppointment { get; set; }

        public ICommand AddNoteCommand => new Command(_ => modalNavigationStore.SetViewModel<AddNoteViewModel>(canClose: true));
        public ICommand StartAppointmentCommand => new AsyncCommand(StartAppointmentAsync);
        public ICommand ToAppointmentCommand =>
            new Command(_ => navigationStore.SetViewModel<AppointmentViewModel>(vm => vm.Appointment = SelectedAppointment!));

        public async Task LoadData()
        {
            Appointments = new(await appointmentRepository.GetPatientAppointmentsAsync(Patient.Id));
            Notes = new(await noteRepository.GetPatientNotesAsync(Patient.Id));
        }

        private async Task StartAppointmentAsync(object? arg)
        {
            var appointment = new Appointment()
            {
                DateTime = DateTime.Now.ToString(),
                Doctor = AppSession.Doctor.Id,
                Patient = AppointmentSession.Patient.Id
            };

            await appointmentRepository.AddAsync(appointment);

            AppointmentSession.Appointment = appointment;

            navigationStore.SetViewModel<AppointmentViewModel>(vm => vm.Appointment = appointment);
        }
    }
}