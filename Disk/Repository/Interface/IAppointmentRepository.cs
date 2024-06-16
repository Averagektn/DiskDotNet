using Disk.Entities;
using Disk.Repository.Common.Interface;
using Serilog;

namespace Disk.Repository.Interface
{
    public interface IAppointmentRepository : ICrudRepository<Appointment> 
    {
        ICollection<Appointment> GetPatientAppointments(long id);
        Task<ICollection<Appointment>> GetPatientAppointmentsAsync(long id);
    }
}
