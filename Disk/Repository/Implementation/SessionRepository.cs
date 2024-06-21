using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Common.Implementation;
using Disk.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Disk.Repository.Implementation
{
    public class SessionRepository(DiskContext diskContext) : CrudRepository<Session>(diskContext), ISessionRepository
    {
        public ICollection<Session> GetSessionsWithResultsByAppointment(long appointmentId)
        {
            return table.Where(s => s.Appointment == appointmentId)
                .Include(s => s.PathInTargets)
                .Include(s => s.PathToTargets)
                .Include(s => s.SessionResult)
                .Include(s => s.MapNavigation)
                .ToList();
        }
    }
}
