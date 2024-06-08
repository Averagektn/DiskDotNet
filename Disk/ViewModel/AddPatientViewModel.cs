using Disk.Entities;
using Disk.Repository.Exceptions;
using Disk.Repository.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Serilog;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class AddPatientViewModel(ModalNavigationStore modalNavigationStore, IPatientRepository patientRepository) : PopupViewModel
    {
        private Patient _patient = new();
        public Patient Patient { get => _patient; set => SetProperty(ref _patient, value); }

        public ICommand AddPatientCommand => new AsyncCommand(AddPatient);
        public ICommand CancelCommand => new Command(Cancel);

        private async Task AddPatient(object? arg)
        {
            try
            {
                await patientRepository.AddAsync(Patient);
            }
            catch (DuplicateEntityException ex)
            {
                Log.Error(ex.Message);
                await ShowPopup("Adding error", "Patient already exists");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
            }

            modalNavigationStore.Close();
        }

        private void Cancel(object? obj)
        {
            modalNavigationStore.Close();
        }
    }
}