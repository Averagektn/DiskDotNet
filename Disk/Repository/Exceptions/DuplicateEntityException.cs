using Disk.Repository.Exceptions.Common;

namespace Disk.Repository.Exceptions
{
    public class DuplicateEntityException : RepositoryException 
    {
        public DuplicateEntityException(string message) : base(message) { }
        public DuplicateEntityException(string output, string message) : base(output, message) { }
    }
}
