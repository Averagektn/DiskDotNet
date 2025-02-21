using Disk.Entities;
using Disk.Repository.Common.Interface;

namespace Disk.Repository.Interface;

public interface ISessionRepository : ICrudRepository<Session>
{
    ICollection<Session> GetSessionsWithResultsByAppointment(long appointmentId);
    bool Exists(Session session);
}
