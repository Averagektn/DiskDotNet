using Disk.Entities;
using Disk.Properties.Langs.AddPatient;
using Disk.Service.Exceptions;
using Disk.Service.Interface;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Serilog;
using System.Windows.Input;
using System.Windows.Media;

namespace Disk.ViewModel
{
    public class EditPatientViewModel(IPatientService patientService) : AddPatientViewModel(patientService)
    {
        public required Patient Backup { get; set; }
        public event Action? AfterUpdateEvent;

        private readonly IPatientService _patientService = patientService;
        public override ICommand AddPatientCommand => new AsyncCommand(UpdatePatient);
        public override ICommand CancelCommand => new Command(
            _ =>
            {
                Patient.Patronymic = Backup.Patronymic;
                Patient.Surname = Backup.Surname;
                Patient.DateOfBirth = Backup.DateOfBirth;
                Patient.Name = Backup.Name;
                Patient.PhoneHome = Backup.PhoneHome;
                Patient.PhoneMobile = Backup.PhoneMobile;

                AfterUpdateEvent?.Invoke();
                base.CancelCommand.Execute(null);
            });

        private async Task UpdatePatient(object? obj)
        {
            bool success = false;

            try
            {
                _patientService.Update(Patient);
                success = true;
            }
            catch (InvalidNameException ex)
            {
                Log.Information(ex.Message);
                BgName = new SolidColorBrush(Colors.Red);
                await ShowPopup(AddPatientLocalization.ErrorHeader, ex.Output);
            }
            catch (InvalidSurnameException ex)
            {
                Log.Information(ex.Message);
                BgSurname = new SolidColorBrush(Colors.Red);
                await ShowPopup(AddPatientLocalization.ErrorHeader, ex.Output);
            }
            catch (InvalidDateException ex)
            {
                Log.Information(ex.Message);
                BgDateOfBirth = new SolidColorBrush(Colors.Red);
                await ShowPopup(AddPatientLocalization.ErrorHeader, ex.Output);
            }
            catch (InvalidPhoneNumberException ex)
            {
                Log.Information(ex.Message);
                BgMobilePhone = new SolidColorBrush(Colors.Red);
                await ShowPopup(AddPatientLocalization.ErrorHeader, ex.Output);
            }
            catch (InvalidHomePhoneException ex)
            {
                Log.Information(ex.Message);
                BgHomePhone = new SolidColorBrush(Colors.Red);
                await ShowPopup(AddPatientLocalization.ErrorHeader, ex.Output);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
                throw;
            }

            if (success)
            {
                IniNavigationStore.Close();
                AfterUpdateEvent?.Invoke();
            }
        }
    }
}
