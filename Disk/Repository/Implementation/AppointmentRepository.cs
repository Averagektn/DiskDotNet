using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Common.Implementation;
using Disk.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Disk.Repository.Implementation
{
    public class AppointmentRepository(DiskContext diskContext) : CrudRepository<Appointment>(diskContext), IAppointmentRepository
    {
        public ICollection<Appointment> GetPatientAppointments(long id)
        {
            return table.Where(a => a.Patient == id).ToList();
        }

        public async Task<ICollection<Appointment>> GetPatientAppointmentsAsync(long id)
        {
            return await table.Where(a => a.Patient == id).ToListAsync();
        }
    }
}
