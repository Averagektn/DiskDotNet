using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Common.Implementation;
using Disk.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Disk.Repository.Implementation;

public class AppointmentRepository(DiskContext diskContext) : CrudRepository<Appointment>(diskContext), IAppointmentRepository
{
    public long GetAppointmentsCount(long patientId)
    {
        return Table.Count();
    }

    public ICollection<Appointment> GetPagedAppointments(long patientId, int page, int appointmentsPerPage)
    {
        return [.. Table
            .Where(a => a.Patient == patientId)
            .OrderByDescending(a => a.Id)
            .Skip(page * appointmentsPerPage)
            .Take(appointmentsPerPage)];
    }

    public ICollection<Appointment> GetPatientAppointments(long id)
    {
        return [.. Table.Where(a => a.Patient == id).OrderByDescending(a => a.Id)];
    }

    public async Task<ICollection<Appointment>> GetPatientAppointmentsAsync(long id)
    {
        return await Table.Where(a => a.Patient == id).OrderByDescending(a => a.Id).ToListAsync();
    }

    public async Task<Appointment?> GetAppointmentWithSessions(long id)
    {
        return await Table
            .Where(a => a.Id == id)
            .Include(a => a.Sessions)
            .Include(a => a.MapNavigation)
            .FirstOrDefaultAsync();
    }
}
