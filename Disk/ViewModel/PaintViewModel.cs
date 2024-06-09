using Disk.Data.Impl;
using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.ViewModels;

namespace Disk.ViewModel
{
    public class PaintViewModel(NavigationStore navigationStore, IPathToTargetRepository pathToTargetRepository, 
        IPathInTargetRepository pathInTargetRepository, ISessionResultRepository sessionResultRepository) : ObserverViewModel
    {
        public string CurrPath { get; set; } = null!;
        private int targetId;
        public List<Point2D<float>> TargetCenters = null!;

        public void NavigateToAppoinment()
        {
            navigationStore.SetViewModel<AppointmentViewModel>(vm => vm.Appointment = AppointmentSession.Appointment);
        }

        public Point2D<float>? GetNextTargetCenter()
        {
            return TargetCenters.Count <= targetId ? null : TargetCenters[targetId++];
        }

        public void SaveSessionResult(SessionResult sessionResult)
        {
            sessionResult.Id = AppointmentSession.CurrentSession.Id;
            sessionResultRepository.Add(sessionResult);
        }

        public void SavePathToTarget(PathToTarget pathToTarget)
        {
            pathToTarget.Session = AppointmentSession.CurrentSession.Id;
            pathToTargetRepository.Add(pathToTarget);
        }

        public void SavePathInTarget(PathInTarget pathInTarget)
        {
            pathInTarget.Session = AppointmentSession.CurrentSession.Id;
            pathInTargetRepository.Add(pathInTarget);
        }
    }
}