using Disk.Entities;
using Disk.Properties.Langs.AddPatient;
using Disk.Service.Exceptions;
using Disk.Service.Interface;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Serilog;
using System.Windows.Input;
using System.Windows.Media;

namespace Disk.ViewModel;

public class EditPatientViewModel(IPatientService patientService) : AddPatientViewModel(patientService)
{
    public event Action? AfterUpdateEvent;
    public required Patient AttachedPatient;
    private readonly IPatientService _patientService = patientService;

    public override ICommand AddPatientCommand => new AsyncCommand(UpdatePatient);

    public override ICommand CancelCommand => new Command(_ =>
    {
        AfterUpdateEvent?.Invoke();
        base.CancelCommand.Execute(null);
    });

    private async Task UpdatePatient(object? obj)
    {
        bool success = false;

        AttachedPatient.Surname = Patient.Surname;
        AttachedPatient.Name = Patient.Name;
        AttachedPatient.Patronymic = Patient.Patronymic;
        AttachedPatient.DateOfBirth = Patient.DateOfBirth;
        AttachedPatient.PhoneHome = Patient.PhoneHome;
        AttachedPatient.PhoneMobile = Patient.PhoneMobile;

        try
        {
            _patientService.Update(AttachedPatient);
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
            Log.Error(ex.Message);
            throw;
        }

        if (success)
        {
            IniNavigationStore.Close();
            AfterUpdateEvent?.Invoke();
        }
    }
}
