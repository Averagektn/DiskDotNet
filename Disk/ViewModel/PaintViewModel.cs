using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.ViewModels;
using Disk.Visual.Impl;

namespace Disk.ViewModel
{
    public class PaintViewModel(NavigationStore navigationStore, IPathToTargetRepository pathToTargetRepository,
        IPathInTargetRepository pathInTargetRepository, ISessionResultRepository sessionResultRepository) : ObserverViewModel, IDisposable
    {
        public event Action? OnSessionOver;
        public string CurrPath { get; set; } = null!;
        public List<Point2D<float>> TargetCenters { get; set; } = null!;
        public bool UserPictureSelected { get; set; } = true;

        public int TargetId { get; set; }

        public void NavigateToAppoinment()
        {
            navigationStore.SetViewModel<AppointmentViewModel>(vm => vm.Appointment = AppointmentSession.Appointment);
        }

        public Point2D<float>? NextTargetCenter => TargetCenters.Count <= TargetId ? null : TargetCenters[TargetId++];

        public void SaveSessionResult(SessionResult sessionResult)
        {
            sessionResult.Id = AppointmentSession.CurrentSession.Id;
            sessionResultRepository.Add(sessionResult);
            OnSessionOver?.Invoke();
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

        public void Dispose()
        {
            
        }
    }
}