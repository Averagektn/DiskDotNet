using Disk.Entities;

namespace Disk.Sessions
{
    public static class AppointmentSession
    {
        public static Patient Patient { get; set; } = new();
    }
}
