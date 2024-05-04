using Disk.Db.Context;
using Disk.Entity;

namespace Disk.Repository
{
    public class DoctorPatientRepository(DiskContext context)
    {
        public async Task<int> AssignAppointment()
        {
            throw new NotImplementedException();
        }

        public async Task AssignProcedure()
        {
            throw new NotImplementedException();
        }

        public async Task AddNote(Note note)
        {
            throw new NotImplementedException();
        }
    }
}
