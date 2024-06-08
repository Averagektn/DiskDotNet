﻿using Disk.Entities;
using Disk.Properties.Langs;
using Disk.Service.Exceptions;
using Disk.Service.Exceptions.Common;
using Disk.Service.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Serilog;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Disk.ViewModel
{
    public class AuthenticationViewModel(IAuthenticationService authenticationService, ModalNavigationStore modalNavigationStore) 
        : PopupViewModel
    {
        public Doctor Doctor { get; set; } = new() { Name = string.Empty, Surname = string.Empty, Password = string.Empty };

        private Brush _bgName = new SolidColorBrush(Colors.White);
        public Brush BgName { get => _bgName; set => SetProperty(ref _bgName, value); }

        private Brush _bgSurname = new SolidColorBrush(Colors.White);
        public Brush BgSurname { get => _bgSurname; set => SetProperty(ref _bgSurname, value); }

        private Brush _bgPassword = new SolidColorBrush(Colors.White);
        public Brush BgPassword { get => _bgPassword; set => SetProperty(ref _bgPassword, value); }

        public ICommand AuthorizationCommand => new AsyncCommand(PerformAuthorization);
        public ICommand RegistrationCommand => new AsyncCommand(PerformRegistration);
        public ICommand ReturnWhiteNameCommand => new Command(ReturnWhiteName);
        public ICommand ReturnWhiteSurnameCommand => new Command(ReturnWhiteSurname);
        public ICommand ReturnWhitePasswordCommand => new Command(ReturnWhitePassword);

        private async Task PerformRegistration(object? param)
        {
            try
            {
                _ = await authenticationService.PerformRegistrationAsync(Doctor);
                Application.Current.Properties["doctor"] = Doctor;
                modalNavigationStore.Close();
            }
            catch (InvalidNameException ex)
            {
                BgName = new SolidColorBrush(Colors.Red);
                await ShowPopupAndLog(ex, AuthenticationLocalization.RegistrationError);
            }
            catch (InvalidSurnameException ex)
            {
                BgSurname = new SolidColorBrush(Colors.Red);
                await ShowPopupAndLog(ex, AuthenticationLocalization.RegistrationError);
            }
            catch (InvalidPasswordException ex)
            {
                BgPassword = new SolidColorBrush(Colors.Red);
                await ShowPopupAndLog(ex, AuthenticationLocalization.RegistrationError);
            }
            catch (ServiceException ex)
            {
                await ShowPopupAndLog(ex, AuthenticationLocalization.RegistrationError);
            }
            catch (Exception ex)
            {
                Log.Fatal("UNKNOWN ERROR", ex.Message, ex.StackTrace, ex.Source, ex.Data, ex);
                throw;
            }
        }

        private async Task PerformAuthorization(object? param)
        {
            bool isSuccessfulAuth = false;
            bool isQuerySent = false;

            try
            {
                isSuccessfulAuth = await authenticationService.PerformAuthorizationAsync(Doctor);
                isQuerySent = true;
            }
            catch (InvalidNameException ex)
            {
                BgName = new SolidColorBrush(Colors.Red);
                await ShowPopupAndLog(ex, AuthenticationLocalization.AuthorizationError);
            }
            catch (InvalidSurnameException ex)
            {
                BgSurname = new SolidColorBrush(Colors.Red);
                await ShowPopupAndLog(ex, AuthenticationLocalization.AuthorizationError);
            }
            catch (InvalidPasswordException ex)
            {
                BgPassword = new SolidColorBrush(Colors.Red);
                await ShowPopupAndLog(ex, AuthenticationLocalization.AuthorizationError);
            }
            catch (ServiceException ex)
            {
                await ShowPopupAndLog(ex, AuthenticationLocalization.AuthorizationError);
            }
            catch (Exception ex)
            {
                Log.Fatal("UNKNOWN ERROR", ex.Message, ex.StackTrace, ex.Source, ex.Data, ex);
                throw;
            }

            if (isQuerySent && isSuccessfulAuth)
            {
                Application.Current.Properties["doctor"] = Doctor;
                modalNavigationStore.Close();
            }
            else if (isQuerySent)
            {
                await ShowPopup(AuthenticationLocalization.AuthorizationError, AuthenticationLocalization.NotFound);
            }
        }

        private async Task ShowPopupAndLog(ServiceException ex, string header)
        {
            await ShowPopup(header, ex.Output);
            Log.Logger.Error(ex.Message);
        }

        private void ReturnWhiteName(object? param) => BgName = new SolidColorBrush(Colors.White);
        private void ReturnWhiteSurname(object? param) => BgSurname = new SolidColorBrush(Colors.White);
        private void ReturnWhitePassword(object? param) => BgPassword = new SolidColorBrush(Colors.White);
    }
}