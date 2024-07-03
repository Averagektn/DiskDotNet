using Disk.Entities;
using Disk.Navigators;
using Disk.Repository.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class PatientsViewModel : ObserverViewModel
    {
        public ICommand AddPatientCommand => new Command(_ =>
        {
            Action<Patient> onAdd = patient => SortedPatients.Add(patient);
            onAdd += _ =>
            {
                PageNum = TotalPages;
                IsPrevEnabled = PageNum > 1;
                IsNextEnabled = PageNum < TotalPages;

                GetPagedPatients();
            };

            AddPatientNavigator.Navigate(_modalNavigationStore, onAdd);
        });
        public ICommand SearchCommand => new Command(Search);
        public ICommand SelectPatientCommand => new Command(SelectPatient);
        public ICommand DeletePatientCommand => new Command(
            _ =>
            {
                if (SelectedPatient is null)
                {
                    return;
                }

                _patientRepository.Delete(SelectedPatient);
                _ = SortedPatients.Remove(SelectedPatient);

                if (SortedPatients.Count == 0 && PageNum > 1)
                {
                    PageNum--;
                }

                IsPrevEnabled = PageNum > 1;
                IsNextEnabled = PageNum < TotalPages;

                SearchText = string.Empty;

                GetPagedPatients();
            });

        public ICommand UpdatePatientCommand => new Command(
            _ =>
            {
                if (SelectedPatient is null)
                {
                    return;
                }

                void OnCancel(Patient patient)
                {
                    var id = SortedPatients.IndexOf(patient);
                    SortedPatients.RemoveAt(id);
                    SortedPatients.Insert(id, _patientRepository.GetById(patient.Id));
                }

                EditPatientNavigator.Navigate(_modalNavigationStore, OnCancel, SelectedPatient);
            });

        public ICommand NextPageCommand => new Command(
            _ =>
            {
                SearchText = string.Empty;

                PageNum++;
                IsNextEnabled = PageNum < TotalPages;
                IsPrevEnabled = true;

                GetPagedPatients();
            });
        public ICommand PrevPageCommand => new Command(
            _ =>
            {
                SearchText = string.Empty;

                PageNum--;
                IsPrevEnabled = PageNum > 1;
                IsNextEnabled = true;

                GetPagedPatients();
            });

        private bool _isNextEnabled;
        public bool IsNextEnabled { get => _isNextEnabled; set => SetProperty(ref _isNextEnabled, value); }

        private bool _isPrevEnabled = false;
        public bool IsPrevEnabled { get => _isPrevEnabled; set => SetProperty(ref _isPrevEnabled, value); }

        private int _pageNum = 1;
        public int PageNum { get => _pageNum; set => SetProperty(ref _pageNum, value); }

        public ObservableCollection<Patient> SortedPatients { get; set; }
        public Patient? SelectedPatient { get; set; }

        private string _searchText = string.Empty;
        public string SearchText { get => _searchText; set => SetProperty(ref _searchText, value); }

        private readonly NavigationStore _navigationStore;
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly IPatientRepository _patientRepository;

        private int TotalPages => (int)float.Ceiling((float)_patientRepository.GetPatientsCount() / PatientsPerPage);
        private const int PatientsPerPage = 18;

        public PatientsViewModel(NavigationStore navigationStore, ModalNavigationStore modalNavigationStore,
            IPatientRepository patientRepository)
        {
            _navigationStore = navigationStore;
            _modalNavigationStore = modalNavigationStore;
            _patientRepository = patientRepository;

            SortedPatients = new(_patientRepository.GetPatientsPage(PageNum - 1, PatientsPerPage));
            IsNextEnabled = (int)float.Ceiling((float)patientRepository.GetPatientsCount() / PatientsPerPage) > 1;
        }

        private void Search(object? arg)
        {
            if (SearchText != string.Empty)
            {
                var nsp = SearchText.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                var surname = nsp[0];
                var name = string.Empty;
                var patronymic = string.Empty;

                if (nsp.Length > 1)
                {
                    name = nsp[1];
                    if (nsp.Length > 2)
                    {
                        patronymic = nsp[2];
                    }
                }
                var patients = _patientRepository.GetPatientsByFullname(name, surname, patronymic);

                SortedPatients.Clear();
                foreach (var patient in patients)
                {
                    SortedPatients.Add(patient);
                }
            }
            else
            {
                SortedPatients.Clear();
                GetPagedPatients();
            }
        }

        private void GetPagedPatients()
        {
            SortedPatients.Clear();
            foreach (var patient in _patientRepository.GetPatientsPage(PageNum - 1, PatientsPerPage))
            {
                SortedPatients.Add(patient);
            }
        }

        private void SelectPatient(object? obj)
        {
            if (SelectedPatient is null)
            {
                return;
            }

            AppointmentsListNavigator.NavigateWithBar(_navigationStore, SelectedPatient);
        }
    }
}