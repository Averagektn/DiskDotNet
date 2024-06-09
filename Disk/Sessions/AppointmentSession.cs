using Disk.Entities;

namespace Disk.Sessions
{
    public static class AppointmentSession
    {
        public static Patient Patient { get; set; } = new();
        public static Appointment Appointment { get; set; } = new();
        public static Session CurrentSession { get; set; } = new();
    }
}
