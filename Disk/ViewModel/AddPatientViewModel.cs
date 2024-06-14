using Disk.Entities;
using Disk.Properties.Langs.AddPatient;
using Disk.Repository.Exceptions;
using Disk.Service.Exceptions;
using Disk.Service.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Serilog;
using System.Windows.Input;
using System.Windows.Media;

namespace Disk.ViewModel
{
    public class AddPatientViewModel(ModalNavigationStore modalNavigationStore, IPatientService patientService) : PopupViewModel
    {
        public event Action<Patient>? OnAdd;

        private Brush _bgName = new SolidColorBrush(Colors.White);
        public Brush BgName { get => _bgName; set => SetProperty(ref _bgName, value); }

        private Brush _bgSurname = new SolidColorBrush(Colors.White);
        public Brush BgSurname { get => _bgSurname; set => SetProperty(ref _bgSurname, value); }

        private Brush _bgDateOfBirth = new SolidColorBrush(Colors.White);
        public Brush BgDateOfBirth { get => _bgDateOfBirth; set => SetProperty(ref _bgDateOfBirth, value); }

        private Brush _bgMobilePhone = new SolidColorBrush(Colors.White);
        public Brush BgMobilePhone { get => _bgMobilePhone; set => SetProperty(ref _bgMobilePhone, value); }

        private Brush _bgHomePhone = new SolidColorBrush(Colors.White);
        public Brush BgHomePhone { get => _bgHomePhone; set => SetProperty(ref _bgHomePhone, value); }

        public ICommand NameFocusCommand => new Command(_ => BgName = new SolidColorBrush(Colors.White));
        public ICommand SurnameFocusCommand => new Command(_ => BgSurname = new SolidColorBrush(Colors.White));
        public ICommand DateOfBirthFocusCommand => new Command(_ => BgDateOfBirth = new SolidColorBrush(Colors.White));
        public ICommand MobilePhoneFocusCommand => new Command(_ => BgMobilePhone = new SolidColorBrush(Colors.White));
        public ICommand HomePhoneFocusCommand => new Command(_ => BgHomePhone = new SolidColorBrush(Colors.White));

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
        public ICommand CancelCommand => new Command(_ => modalNavigationStore.Close());

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
                await ShowPopup(AddPatientLocalization.ErrorHeader, AddPatientLocalization.Duplication);
            }
            catch (InvalidNameException ex)
            {
                Log.Error(ex.Message);
                BgName = new SolidColorBrush(Colors.Red);
                await ShowPopup(AddPatientLocalization.ErrorHeader, AddPatientLocalization.EmptyName);
            }
            catch (InvalidSurnameException ex)
            {
                Log.Error(ex.Message);
                BgSurname = new SolidColorBrush(Colors.Red);
                await ShowPopup(AddPatientLocalization.ErrorHeader, AddPatientLocalization.EmptySurname);
            }
            catch (InvalidDateException ex)
            {
                Log.Error(ex.Message);
                BgDateOfBirth = new SolidColorBrush(Colors.Red);
                await ShowPopup(AddPatientLocalization.ErrorHeader, AddPatientLocalization.InvalidDate);
            }
            catch (InvalidPhoneNumberException ex)
            {
                Log.Error(ex.Message);
                BgMobilePhone = new SolidColorBrush(Colors.Red);
                await ShowPopup(AddPatientLocalization.ErrorHeader, AddPatientLocalization.InvalidMobilePhone);
            }
            catch (InvalidHomePhoneException ex)
            {
                Log.Error(ex.Message);
                BgHomePhone = new SolidColorBrush(Colors.Red);
                await ShowPopup(AddPatientLocalization.ErrorHeader, AddPatientLocalization.InvalidHomePhone);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
                throw;
            }

            if (success)
            {
                OnAdd?.Invoke(Patient);
                modalNavigationStore.Close();
            }
        }
    }
}