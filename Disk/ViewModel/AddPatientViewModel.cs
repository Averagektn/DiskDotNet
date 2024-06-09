using Disk.Entities;
using Disk.Repository.Exceptions;
using Disk.Service.Exceptions;
using Disk.Service.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Serilog;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class AddPatientViewModel(NavigationStore navigationStore, ModalNavigationStore modalNavigationStore,
        IPatientService patientService) : PopupViewModel
    {
        private Patient _patient = new()
        {
            DateOfBirth = string.Empty,
            Name = string.Empty,
            PhoneHome = string.Empty,
            PhoneMobile = string.Empty,
            Surname = string.Empty
        };
        public Patient Patient { get => _patient; set => SetProperty(ref _patient, value); }

        public ICommand AddPatientCommand => new AsyncCommand(AddPatient);
        public ICommand CancelCommand => new Command(Cancel);

        private async Task AddPatient(object? arg)
        {
            bool success = false;

            try
            {
                await patientService.AddPatientAsync(Patient);
                success = true;
            }
            catch (DuplicateEntityException ex)
            {
                Log.Error(ex.Message);
                await ShowPopup("Adding error", "Patient already exists");
            }
            catch (InvalidNameException ex)
            {
                Log.Error(ex.Message);
                await ShowPopup("Adding error", "Patient already exists");
            }
            catch (InvalidSurnameException ex)
            {
                Log.Error(ex.Message);
                await ShowPopup("Adding error", "Patient already exists");
            }
            catch (InvalidDateException ex)
            {
                Log.Error(ex.Message);
                await ShowPopup("Adding error", "Patient already exists");
            }
            catch (InvalidPhoneNumberException ex)
            {
                Log.Error(ex.Message);
                await ShowPopup("Adding error", "Patient already exists");
            }
            catch (InvalidHomePhoneException ex)
            {
                Log.Error(ex.Message);
                await ShowPopup("Adding error", "Patient already exists");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
                throw;
            }

            if (success)
            {
                _ = navigationStore.NavigateBack();
                navigationStore.SetViewModel<PatientsViewModel>();
                modalNavigationStore.Close();
            }
        }

        private void Cancel(object? obj)
        {
            modalNavigationStore.Close();
        }
    }
}