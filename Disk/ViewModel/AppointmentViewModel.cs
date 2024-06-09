using Disk.Entities;
using Disk.ViewModel.Common.ViewModels;

namespace Disk.ViewModel
{
    public class AppointmentViewModel : ObserverViewModel
    {
        public Appointment Appointment { get; set; } = null!;
    }
}