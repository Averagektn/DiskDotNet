using Disk.Entities;

namespace Disk.Sessions
{
    public static class AppSession
    {
        public static Doctor Doctor { get; set; } = new();
    }
}
