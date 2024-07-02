using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Common.Implementation;
using Disk.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Disk.Repository.Implementation
{
    public class AppointmentRepository(DiskContext diskContext) : CrudRepository<Appointment>(diskContext), IAppointmentRepository
    {
        public long GetAppointmentsCount(long patientId)
        {
            return table.Count();
        }

        public ICollection<Appointment> GetAppoitmentsByDate(long patientId, DateTime date)
        {
            var appointments = table
                .Where(a => a.Patient == patientId)
                .ToList();

            return appointments
                .Where(a => DateTime.ParseExact(a.DateTime, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture).Date.Equals(date.Date))
                .ToList();
        }

        public ICollection<Appointment> GetPagedAppointments(long patientId, int page, int appointmentsPerPage)
        {
            return table
                .Where(a => a.Patient == patientId)
                .Skip(page * appointmentsPerPage)
                .Take(appointmentsPerPage)
                .ToList();
        }

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
