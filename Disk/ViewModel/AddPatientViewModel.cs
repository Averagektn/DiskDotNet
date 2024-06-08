using Disk.Entities;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class AddPatientViewModel(ModalNavigationStore modalNavigationStore) : PopupViewModel
    {
        private Patient _patient = new();
        public Patient Patient { get => _patient; set => SetProperty(ref _patient, value); }

        public ICommand AddPatientCommand => new Command(AddPatient);

        private void AddPatient(object? obj)
        {

        }
    }
}