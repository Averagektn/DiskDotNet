using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.ViewModels;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class AuthenticationViewModel : PopupViewModel
    {
        public Doctor Doctor { get; set; } = new();

        public ICommand AuthorizationCommand { get; set; }
        public ICommand RegistrationCommand { get; set; }

        private readonly NavigationStore _navigationStore;
        private readonly IDoctorRepository _doctorRepository;

        public AuthenticationViewModel(IDoctorRepository doctorRepository, NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _doctorRepository = doctorRepository;

            AuthorizationCommand = new AsyncCommand(PerformAuthorization);
            RegistrationCommand = new AsyncCommand(PerformRegistration);
        }

        private async Task PerformRegistration(object? param)
        {
            _doctorRepository.PerformRegistration(Doctor);
            _navigationStore.CurrentViewModel = new MenuViewModel();
        }

        private async Task PerformAuthorization(object? param)
        {
            _doctorRepository.PerformAuthorization(Doctor);
            _navigationStore.CurrentViewModel = new MenuViewModel();
        }
    }
}