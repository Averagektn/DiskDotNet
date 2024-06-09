using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Common.Implementation;
using Disk.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Disk.Repository.Implementation
{
    public class NoteRepository(DiskContext diskContext) : CrudRepository<Note>(diskContext), INoteRepository
    {
        public async Task<ICollection<Note>> GetPatientNotesAsync(long patientId)
        {
            return await table.Where(n => n.Patient == patientId).ToListAsync();
        }

        public ICollection<Note> GetPatientNotes(long patientId)
        {
            return table.Where(n => n.Patient == patientId).ToList();
        }
    }
}
