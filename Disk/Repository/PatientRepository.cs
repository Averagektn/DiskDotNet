using Disk.Db.Context;
using Disk.Entity;

namespace Disk.Repository
{
    public class PatientRepository(DiskContext context)
    {
        public async Task<int> AddPatient(Patient patient)
        {
            throw new NotImplementedException();
        }

        public async Task AddCard()
        {
            throw new NotImplementedException();
        }

        public async Task AddAddress()
        {
            throw new NotImplementedException();
        }

        public async Task AddContraindication(Contraindication contraindication)
        {
            throw new NotImplementedException();
        }

        public async Task AddXray(Xray xray)
        {
            throw new NotImplementedException();
        }

        public async Task AddDiagnosis(Diagnosis diagnosis)
        {
            throw new NotImplementedException();
        }
    }
}
