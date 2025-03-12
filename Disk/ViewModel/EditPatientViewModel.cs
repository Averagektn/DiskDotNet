using Disk.Db.Context;
using Disk.Navigators;
using Disk.Properties.Langs.EditPatient;
using Disk.Service.Exceptions;
using Disk.Service.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Serilog;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Disk.ViewModel;

public class EditPatientViewModel(IPatientService patientService, ModalNavigationStore modalNavigationStore, DiskContext database)
    : AddPatientViewModel(patientService, modalNavigationStore, database)
{
    private readonly IPatientService _patientService = patientService;
    private readonly ModalNavigationStore _modalNavigationStore = modalNavigationStore;
    private readonly DiskContext _database = database;

    public override ICommand CancelCommand => new AsyncCommand(async _ =>
    {
        await _database.Entry(Patient).ReloadAsync();
        base.CancelCommand.Execute(null);
    });

    public override ICommand AddPatientCommand => new AsyncCommand(async _ =>
    {
        bool validated = false;

        try
        {
            await _patientService.CheckDuplicateAndUpdateAsync(Patient);
            validated = true;
        }
        catch (DuplicateEntityException ex)
        {
            Log.Error(ex.Message);
            await _database.Entry(Patient).ReloadAsync();
            await ShowPopup(EditPatientLocalization.ErrorHeader, EditPatientLocalization.Duplication);
        }
        catch (PossibleDuplicateEntityException ex)
        {
            Log.Information(ex.Message);
            QuestionNavigator.Navigate(_modalNavigationStore,
                message: EditPatientLocalization.PossibleDuplication,
                beforeConfirm: () =>
                {
                    _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        _ = _database.Update(Patient);
                        _ = await _database.SaveChangesAsync();
                        IniNavigationStore.Close();
                    });
                },
                beforeCancel: () =>
                    _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                        await _database.Entry(Patient).ReloadAsync()));
        }
        catch (InvalidNameException ex)
        {
            Log.Information(ex.Message);
            BgName = Brushes.Red;
            await _database.Entry(Patient).ReloadAsync();
            await ShowPopup(EditPatientLocalization.ErrorHeader, ex.Output);
        }
        catch (InvalidSurnameException ex)
        {
            Log.Information(ex.Message);
            BgSurname = Brushes.Red;
            await _database.Entry(Patient).ReloadAsync();
            await ShowPopup(EditPatientLocalization.ErrorHeader, ex.Output);
        }
        catch (InvalidDateException ex)
        {
            Log.Information(ex.Message);
            BgDateOfBirth = Brushes.Red;
            await _database.Entry(Patient).ReloadAsync();
            await ShowPopup(EditPatientLocalization.ErrorHeader, ex.Output);
        }
        catch (InvalidPhoneNumberException ex)
        {
            Log.Information(ex.Message);
            BgMobilePhone = Brushes.Red;
            await _database.Entry(Patient).ReloadAsync();
            await ShowPopup(EditPatientLocalization.ErrorHeader, ex.Output);
        }
        catch (InvalidHomePhoneException ex)
        {
            Log.Information(ex.Message);
            BgHomePhone = Brushes.Red;
            await _database.Entry(Patient).ReloadAsync();
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
