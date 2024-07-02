using Disk.Data.Impl;
using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Service.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class AppointmentViewModel(ModalNavigationStore modalNavigationStore, ISessionRepository sessionRepository, IExcelFiller excelFiller,
        NavigationStore navigationStore) : ObserverViewModel
    {
        public bool IsNewAppointment { get; set; }
        public Patient Patient { get; set; } = null!;
        public Appointment Appointment { get; set; } = null!;
        public Session? SelectedSession { get; set; }
        public ObservableCollection<Session> Sessions { get; set; } = [];
        public ObservableCollection<PathToTarget> PathsToTargets { get; set; } = [];

        public ICommand StartSessionCommand
            => new Command(_ => modalNavigationStore.SetViewModel<StartSessionViewModel>(
                vm =>
                {
                    vm.OnSessionOver += Update;
                    vm.Appointment = Appointment;
                    vm.Patient = Patient;
                },
                canClose: true));
        public ICommand SessionSelectedCommand => new Command(SessionSelected);
        public ICommand ExportToExcelCommand => new Command(_ => excelFiller.ExportToExcel(Sessions, Patient));
        public ICommand ShowSessionCommand => new Command(ShowSession);
        public ICommand DeleteSessionCommand => new Command(_ =>
        {
            sessionRepository.Delete(SelectedSession!);
            Sessions.Remove(SelectedSession!);
            OnPropertyChanged(nameof(Sessions));
            PathsToTargets.Clear();

            SelectedSession = null;
        });

        private void ShowSession(object? obj)
        {
            if (SelectedSession is null)
            {
                return;
            }

            navigationStore.SetViewModel<PaintViewModel>(vm =>
            {
                vm.PathsToTargets = SelectedSession!.PathToTargets
                    .Select(path => JsonConvert.DeserializeObject<List<Point2D<float>>>(path.CoordinatesJson)!)
                    .ToList();
                vm.PathsInTargets = SelectedSession!.PathInTargets
                    .Select(path => JsonConvert.DeserializeObject<List<Point2D<float>>>(path.CoordinatesJson)!)
                    .ToList();
                vm.TargetCenters = JsonConvert.DeserializeObject<List<Point2D<float>>>(SelectedSession.MapNavigation.CoordinatesJson) ?? [];
                vm.IsGame = false;

                vm.ScoreVisibility = Visibility.Hidden;

                vm.IsBackEnabled = true;
                vm.IsStopEnabled = false;

                vm.CurrentSession = SelectedSession;

                vm.FillTargetsComboBox();
            });
        }

        private void SessionSelected(object? obj)
        {
            if (SelectedSession is null)
            {
                return;
            }

            PathsToTargets.Clear();

            foreach (var pathToTarget in SelectedSession.PathToTargets)
            {
                PathsToTargets.Add(pathToTarget);
            }
        }

        private void Update()
        {
            Sessions.Clear();
            var sessions = sessionRepository.GetSessionsWithResultsByAppointment(Appointment.Id);

            foreach (var session in sessions)
            {
                Sessions.Add(session);
            }
        }
    }
}