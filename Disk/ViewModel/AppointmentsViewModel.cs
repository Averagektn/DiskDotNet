using Disk.Entities;
using Disk.ViewModel.Common.ViewModels;

namespace Disk.ViewModel
{
    public class AppointmentsViewModel : ObserverViewModel
    {
        public Patient Patient { get; set; }
    }
}