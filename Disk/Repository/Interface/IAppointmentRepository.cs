using Disk.Entities;
using Disk.Repository.Common.Interface;

namespace Disk.Repository.Interface;

public interface IAppointmentRepository : ICrudRepository<Appointment>
{
    ICollection<Appointment> GetPatientAppointments(long id);
    Task<ICollection<Appointment>> GetPatientAppointmentsAsync(long id);
    ICollection<Appointment> GetAppoitmentsByDate(long patientId, DateTime date);
    ICollection<Appointment> GetPagedAppointments(long patientId, int page, int appointmentsPerPage);
    long GetAppointmentsCount(long patientId);
}
