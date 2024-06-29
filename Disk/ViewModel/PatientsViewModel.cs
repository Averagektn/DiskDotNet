using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class PatientsViewModel : ObserverViewModel
    {
        public ICommand AddPatientCommand
            => new Command(_ => _modalNavigationStore.SetViewModel<AddPatientViewModel>
            (
                vm =>
                    vm.OnAddEvent += patient => SortedPatients.Add(patient),
                canClose: true)
            );
        public ICommand SearchCommand => new Command(Search);
        public ICommand SelectPatientCommand => new Command(SelectPatient);
        public ICommand DeletePatientCommand => new Command(
            _ =>
            {
                var patient = SelectedPatient!;
                _ = Patients.Remove(patient);
                _ = SortedPatients.Remove(patient);
                _patientRepository.Delete(patient);
            });
        public ICommand UpdatePatientCommand => new Command(
            _ => _modalNavigationStore.SetViewModel<EditPatientViewModel>(
                vm =>
                {
                    vm.Backup = JsonConvert.DeserializeObject<Patient>(JsonConvert.SerializeObject(SelectedPatient))!;
                    vm.Patient = SelectedPatient!;
                    vm.OnCancelEvent += patient =>
                    {
                        var id = SortedPatients.IndexOf(patient);
                        SortedPatients.RemoveAt(id);
                        SortedPatients.Insert(id, _patientRepository.GetById(patient.Id));
                    };
                }));
        public ICommand NextPageCommand => new Command(_ => MessageBox.Show("Next"));
        public ICommand PrevPageCommand => new Command(_ => MessageBox.Show("Prev"));

        private bool _isNextEnabled = true;
        public bool IsNextEnabled { get => _isNextEnabled; set => SetProperty(ref _isNextEnabled, value); }

        private bool _isPrevEnabled = false;
        public bool IsPrevEnabled { get => _isPrevEnabled; set => SetProperty(ref _isPrevEnabled, value); }

        private int _pageNum = 1;
        public int PageNum { get => _pageNum; set => SetProperty(ref _pageNum, value); }

        public ObservableCollection<Patient> SortedPatients { get; set; }
        public List<Patient> Patients => _patientRepository.GetAll().ToList();
        public Patient? SelectedPatient { get; set; }
        public string SearchText { get; set; } = string.Empty;

        private readonly NavigationStore _navigationStore;
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly IPatientRepository _patientRepository;

        private const int PatientsPerPage = 10;

        public PatientsViewModel(NavigationStore navigationStore, ModalNavigationStore modalNavigationStore,
            IPatientRepository patientRepository)
        {
            _navigationStore = navigationStore;
            _modalNavigationStore = modalNavigationStore;
            _patientRepository = patientRepository;

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
            _navigationStore.SetViewModel<NavigationBarLayoutViewModel>(
                vm => vm.CurrentViewModel = _navigationStore.GetViewModel<AppointmentsListViewModel>(
                    vm => vm.Patient = SelectedPatient!
                    )
                );
        }
    }
}