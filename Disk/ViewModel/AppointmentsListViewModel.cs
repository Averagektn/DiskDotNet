using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class AppointmentsListViewModel(NavigationStore navigationStore, IAppointmentRepository appointmentRepository, ISessionRepository sessionRepository)
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
                navigationStore.SetViewModel<NavigationBarLayoutViewModel>(
                    vm => vm.CurrentViewModel = navigationStore.GetViewModel<AppointmentViewModel>(
                        vm =>
                        {
                            vm.Appointment = SelectedAppointment!;
                            vm.Patient = Patient;
                            vm.Sessions = new ObservableCollection<Session>(sessionRepository.GetSessionsWithResultsByAppointment(SelectedAppointment!.Id));
                        }
                    )
                );
            });

        private async Task StartAppointmentAsync(object? arg)
        {
            var appointment = new Appointment()
            {
                DateTime = DateTime.Now.ToString(),
                Patient = Patient.Id
            };

            await appointmentRepository.AddAsync(appointment);

            navigationStore.SetViewModel<NavigationBarLayoutViewModel>(
                vm => vm.CurrentViewModel = navigationStore.GetViewModel<AppointmentViewModel>(
                    vm =>
                    {
                        vm.IsNewAppointment = true;
                        vm.Appointment = appointment;
                        vm.Patient = Patient;
                        vm.Sessions = new ObservableCollection<Session>(sessionRepository.GetSessionsWithResultsByAppointment(appointment.Id));
                    }
                )
            );
        }
    }
}