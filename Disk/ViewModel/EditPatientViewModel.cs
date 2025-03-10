using Disk.Db.Context;
using Disk.Entities;
using Disk.Navigators;
using Disk.Properties.Langs.EditPatient;
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
        bool validated = false;

        AttachedPatient.Surname = Patient.Surname;
        AttachedPatient.Name = Patient.Name;
        AttachedPatient.Patronymic = Patient.Patronymic;
        AttachedPatient.DateOfBirth = Patient.DateOfBirth;
        AttachedPatient.PhoneHome = Patient.PhoneHome;
        AttachedPatient.PhoneMobile = Patient.PhoneMobile;

        try
        {
            await _patientService.UpdateAsync(AttachedPatient);
            validated = true;
        }
        catch (DbUpdateException ex)
        {
            Log.Error(ex.Message);
            BgMobilePhone = Brushes.Red;
            _database.Entry(AttachedPatient).Reload();
            await ShowPopup(EditPatientLocalization.ErrorHeader, EditPatientLocalization.Duplication);
        }
        catch (PossibleDuplicateEntityException ex)
        {
            Log.Information(ex.Message);
            QuestionNavigator.Navigate(_modalNavigationStore,
                message: EditPatientLocalization.PossibleDuplication,
                onConfirm: () =>
                {
                    _ = _database.Update(AttachedPatient);
                    _ = _database.SaveChanges();
                    IniNavigationStore.Close();
                },
                onCancel: null);
        }
        catch (InvalidNameException ex)
        {
            Log.Information(ex.Message);
            BgName = Brushes.Red;
            await ShowPopup(EditPatientLocalization.ErrorHeader, ex.Output);
        }
        catch (InvalidSurnameException ex)
        {
            Log.Information(ex.Message);
            BgSurname = Brushes.Red;
            await ShowPopup(EditPatientLocalization.ErrorHeader, ex.Output);
        }
        catch (InvalidDateException ex)
        {
            Log.Information(ex.Message);
            BgDateOfBirth = Brushes.Red;
            await ShowPopup(EditPatientLocalization.ErrorHeader, ex.Output);
        }
        catch (InvalidPhoneNumberException ex)
        {
            Log.Information(ex.Message);
            BgMobilePhone = Brushes.Red;
            await ShowPopup(EditPatientLocalization.ErrorHeader, ex.Output);
        }
        catch (InvalidHomePhoneException ex)
        {
            Log.Information(ex.Message);
            BgHomePhone = Brushes.Red;
            await ShowPopup(EditPatientLocalization.ErrorHeader, ex.Output);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            throw;
        }

        if (validated)
        {
            IniNavigationStore.Close();
        }
    });
}
