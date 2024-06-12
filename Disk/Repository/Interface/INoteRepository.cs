using Disk.Entities;
using Disk.Repository.Common.Interface;

namespace Disk.Repository.Interface
{
    public interface INoteRepository : ICrudRepository<Note>
    {
        ICollection<Note> GetPatientNotes(long patientId);
        Task<ICollection<Note>> GetPatientNotesAsync(long patientId);
    }
}
