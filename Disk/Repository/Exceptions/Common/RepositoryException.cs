using Disk.Exceptions;

namespace Disk.Repository.Exceptions.Common
{
    public class RepositoryException : BaseException 
    {
        public RepositoryException(string message) : base(message) { }
        public RepositoryException(string output, string message) : base(output, message) { }
    }
}
