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
    // ADD RELOAD BUTTON
    public class PatientsViewModel : ObserverViewModel
    {
        public ICommand AddPatientCommand => new Command(ToAddPatient);
        public ICommand SearchCommand => new Command(Search);
        public ICommand SelectPatientCommand => new Command(SelectPatient);

        public ObservableCollection<Patient> SortedPatients { get; set; } = [];
        public List<Patient> Patients { get; set; }
        public Patient? SelectedPatient { get; set; }
        public string SearchText { get; set; } = string.Empty;

        private readonly NavigationStore _navigationStore;
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly IPatientRepository _patientRepository;

        public PatientsViewModel(NavigationStore navigationStore, ModalNavigationStore modalNavigationStore, IPatientRepository patientRepository)
        {
            _navigationStore = navigationStore;
            _modalNavigationStore = modalNavigationStore;
            _patientRepository = patientRepository;

            Patients = _patientRepository.GetAll().ToList();
            foreach (var patient in Patients)
            {
                SortedPatients.Add(patient);
            }
        }

        private void ToAddPatient(object? arg)
        {
            _modalNavigationStore.SetViewModel<AddPatientViewModel>(canClose: true);
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
            _navigationStore.SetViewModel<AppointmentsViewModel>();
        }
    }
}