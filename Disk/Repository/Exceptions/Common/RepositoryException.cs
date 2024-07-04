using Disk.Exceptions;

namespace Disk.Repository.Exceptions.Common
{
    public class RepositoryException : BaseException
    {
        public RepositoryException(string message) : base(message) { }
        public RepositoryException(string message, string output) : base(message, output) { }
    }
}
