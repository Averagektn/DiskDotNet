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
    public class AppointmentsListViewModel(NavigationStore navigationStore, IAppointmentRepository appointmentRepository) 
        : ObserverViewModel
    {
        public Patient Patient { get; set; } = null!;

        public ObservableCollection<Appointment> Appointments 
            => new(appointmentRepository.GetPatientAppointments(Patient.Id).OrderByDescending(a => DateTime.Parse(a.DateTime)));

        public Appointment? SelectedAppointment { get; set; }

        public ICommand StartAppointmentCommand => new AsyncCommand(StartAppointmentAsync);
        public ICommand ToAppointmentCommand =>
            new Command(_ => 
            {
                AppointmentSession.Appointment = SelectedAppointment!;

                navigationStore.SetViewModel<NavigateBackViewModel>(
                    vm => vm.CurrentViewModel = navigationStore.GetViewModel<AppointmentViewModel>()
                );
            });

        private async Task StartAppointmentAsync(object? arg)
        {
            var appointment = new Appointment()
            {
                DateTime = DateTime.Now.ToString(),
                Patient = AppointmentSession.Patient.Id
            };

            await appointmentRepository.AddAsync(appointment);

            AppointmentSession.Appointment = appointment;

            navigationStore.SetViewModel<NavigateBackViewModel>(
                vm => vm.CurrentViewModel = navigationStore.GetViewModel<AppointmentViewModel>(
                    vm => vm.IsNewAppointment = true
                )
            );
        }
    }
}