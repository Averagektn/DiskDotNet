using Disk.Db.Context;
using Disk.Entities;
using Disk.Repository.Interface;

namespace Disk.Repository.Implementation
{
    public class NoteRepository(DiskContext diskContext) : INoteRepository
    {
        public void Add(Note entity)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(Note entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Note entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Note entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(long id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Note Get(Note entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<Note> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Note>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Note> GetAsync(Note entity)
        {
            throw new NotImplementedException();
        }

        public Note GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<Note> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public void Update(Note entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Note entity)
        {
            throw new NotImplementedException();
        }
    }
}
