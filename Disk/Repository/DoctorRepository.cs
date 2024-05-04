using Disk.Db.Context;
using Disk.Entity;

namespace Disk.Repository
{
    public class DoctorRepository(DiskContext context)
    {
        public async Task<int> PerformRegistration(Doctor doctor)
        {
            throw new NotImplementedException();
        }

        public async Task<int> PerformAuthorization(Doctor doctor)
        {
            throw new NotImplementedException();
        }

        public async Task EditAccount(Doctor doctor)
        {
            throw new NotImplementedException();
        }

        public async Task AddMap(Map map)
        {
            throw new NotImplementedException();
        }

        public async Task AddTargetFile(TargetFile targetFile)
        {
            throw new NotImplementedException();
        }

        public async Task AssignProcedure(Procedure procedure)
        {
            throw new NotImplementedException();
        }
    }
}
