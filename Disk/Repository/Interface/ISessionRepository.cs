using Disk.Entities;
using Disk.Repository.Common;

namespace Disk.Repository.Interface
{
    public interface ISessionRepository : ICrudRepository<Session> 
    {
        ICollection<Session> GetSessionsWithResultsByAppointment(long appointmentId);
    }
}
