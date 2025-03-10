using Disk.Db.Context;
using Disk.Entities;
using Disk.Navigators;
using Disk.Properties.Langs.AddPatient;
using Disk.Service.Exceptions;
using Disk.Service.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Windows.Input;
using System.Windows.Media;

namespace Disk.ViewModel;

public class EditPatientViewModel(IPatientService patientService, ModalNavigationStore modalNavigationStore, DiskContext database) 
    : AddPatientViewModel(patientService, modalNavigationStore, database)
{
    public required Patient AttachedPatient;
    private readonly IPatientService _patientService = patientService;
    private readonly ModalNavigationStore _modalNavigationStore = modalNavigationStore;
    private readonly DiskContext _database = database;
    public override ICommand AddPatientCommand => new AsyncCommand(async _ =>
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
        catch (DbUpdateException ex)
        {
            Log.Error(ex.Message);
            await ShowPopup(AddPatientLocalization.ErrorHeader, AddPatientLocalization.Duplication);
        }
        catch (PossibleDuplicateEntityException ex)
        {
            Log.Information(ex.Message);
            QuestionNavigator.Navigate(_modalNavigationStore, message: "Possible duplicate",
                onConfirm: () => { _database.Update(Patient); IniNavigationStore.Close(); },
                onCancel: null);
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
        }
    });
}
