using Disk.Entities;
using Disk.Repository.Common.Interface;

namespace Disk.Repository.Interface
{
    public interface IPatientRepository : ICrudRepository<Patient> 
    { 
        int GetPatientsCount();
        ICollection<Patient> GetPatientsByFullname(string name, string surname, string patronymic);
        ICollection<Patient> GetPatientsPage(int pageNum, int patientsPerPage);
    }
}
