using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class PatientsViewModel : ObserverViewModel
    {
        public ICommand AddPatientCommand 
            => new Command(_ => _modalNavigationStore.SetViewModel<AddPatientViewModel>(canClose: true));
        public ICommand SearchCommand => new Command(Search);
        public ICommand SelectPatientCommand => new Command(SelectPatient);

        public ObservableCollection<Patient> SortedPatients { get; set; }
        public List<Patient> Patients { get; set; }
        public Patient? SelectedPatient { get; set; }
        public string SearchText { get; set; } = string.Empty;

        private readonly NavigationStore _navigationStore;
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly IPatientRepository _patientRepository;

        public PatientsViewModel(NavigationStore navigationStore, ModalNavigationStore modalNavigationStore, 
            IPatientRepository patientRepository)
        {
            _navigationStore = navigationStore;
            _modalNavigationStore = modalNavigationStore;
            _patientRepository = patientRepository;

            Patients = _patientRepository.GetAll().ToList();
            SortedPatients = new(Patients);
        }

        private void Search(object? arg)
        {
            SortedPatients.Clear();
            var patients = Patients.Where(p =>
                $"{p.Surname} {p.Name} {p.Patronymic}".Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var patient in patients)
            {
                SortedPatients.Add(patient);
            }
        }

        private void SelectPatient(object? obj)
        {
            AppointmentSession.Patient = SelectedPatient!;
            _navigationStore.SetViewModel<AppointmentsListViewModel>(vm => vm.Patient = SelectedPatient!);
        }
    }
}