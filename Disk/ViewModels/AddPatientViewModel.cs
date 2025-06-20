﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Disk.Db.Context;
using Disk.Entities;
using Disk.Navigators;
using Disk.Properties.Langs.AddPatient;
using Disk.Services.Exceptions;
using Disk.Services.Interfaces;
using Disk.Stores;
using Disk.ViewModels.Common.Commands.Async;
using Disk.ViewModels.Common.Commands.Sync;
using Disk.ViewModels.Common.ViewModels;

using Serilog;

namespace Disk.ViewModels;

public class AddPatientViewModel(IPatientService patientService, ModalNavigationStore modalNavigationStore, DiskContext database)
    : PopupViewModel
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

    private DateTime? _dateOfBirth = null;
    public DateTime? DateOfBirth
    {
        get => _dateOfBirth;
        set
        {
            _ = SetProperty(ref _dateOfBirth, value);

            if (value is not null)
            {
                Patient.DateOfBirth = value.Value.ToString("dd.MM.yyyy");
            }
        }
    }

    private Brush _bgName = Brushes.White;
    public Brush BgName { get => _bgName; set => SetProperty(ref _bgName, value); }

    private Brush _bgSurname = Brushes.White;
    public Brush BgSurname { get => _bgSurname; set => SetProperty(ref _bgSurname, value); }

    private Brush _bgDateOfBirth = Brushes.White;
    public Brush BgDateOfBirth { get => _bgDateOfBirth; set => SetProperty(ref _bgDateOfBirth, value); }

    private Brush _bgMobilePhone = Brushes.White;
    public Brush BgMobilePhone { get => _bgMobilePhone; set => SetProperty(ref _bgMobilePhone, value); }

    private Brush _bgHomePhone = Brushes.White;
    public Brush BgHomePhone { get => _bgHomePhone; set => SetProperty(ref _bgHomePhone, value); }

    public ICommand NameFocusCommand => new Command(_ => BgName = Brushes.White);

    public ICommand SurnameFocusCommand => new Command(_ => BgSurname = Brushes.White);

    public ICommand DateOfBirthFocusCommand => new Command(_ => BgDateOfBirth = Brushes.White);

    public ICommand MobilePhoneFocusCommand => new Command(_ => BgMobilePhone = Brushes.White);

    public ICommand HomePhoneFocusCommand => new Command(_ => BgHomePhone = Brushes.White);

    public virtual ICommand CancelCommand => new Command(_ => IniNavigationStore.Close());

    public virtual ICommand AddPatientCommand => new AsyncCommand(async _ =>
    {
        bool validated = false;
        if (Patient.PhoneHome == string.Empty)
        {
            Patient.PhoneHome = null;
        }

        try
        {
            await patientService.CheckDuplicateAndAddAsync(Patient);
            validated = true;
        }
        catch (DuplicateEntityException ex)
        {
            Log.Error(ex.Message);
            await ShowPopup(AddPatientLocalization.ErrorHeader, AddPatientLocalization.Duplication);
        }
        catch (PossibleDuplicateEntityException ex)
        {
            Log.Error(ex.Message);
            QuestionNavigator.Navigate(this, modalNavigationStore,
                message: AddPatientLocalization.PossibleDuplication,
                beforeConfirm: () =>
                {
                    _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        _ = await database.AddAsync(Patient);
                        _ = await database.SaveChangesAsync();
                        IniNavigationStore.Close();
                        Log.Information("Patinent added");
                    }).Task.ContinueWith(e =>
                    {
                        if (e.Exception is not null)
                        {
                            Log.Error($"{e.Exception.Message} \n {e.Exception.StackTrace}");
                        }
                    });
                });
        }
        catch (InvalidNameException ex)
        {
            Log.Error(ex.Message);
            BgName = Brushes.Red;
            await ShowPopup(AddPatientLocalization.ErrorHeader, ex.Output);
        }
        catch (InvalidSurnameException ex)
        {
            Log.Error(ex.Message);
            BgSurname = Brushes.Red;
            await ShowPopup(AddPatientLocalization.ErrorHeader, ex.Output);
        }
        catch (InvalidDateException ex)
        {
            Log.Error(ex.Message);
            BgDateOfBirth = Brushes.Red;
            await ShowPopup(AddPatientLocalization.ErrorHeader, ex.Output);
        }
        catch (InvalidPhoneNumberException ex)
        {
            Log.Error(ex.Message);
            BgMobilePhone = Brushes.Red;
            await ShowPopup(AddPatientLocalization.ErrorHeader, ex.Output);
        }
        catch (InvalidHomePhoneException ex)
        {
            Log.Error(ex.Message);
            BgHomePhone = Brushes.Red;
            await ShowPopup(AddPatientLocalization.ErrorHeader, ex.Output);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex.Message);
            throw;
        }

        if (validated)
        {
            IniNavigationStore.Close();
        }
    });
}
