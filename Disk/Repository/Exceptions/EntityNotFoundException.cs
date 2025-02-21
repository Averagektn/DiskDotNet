using Disk.Repository.Exceptions.Common;

namespace Disk.Repository.Exceptions;

public class EntityNotFoundException : RepositoryException
{
    public EntityNotFoundException(string message) : base(message) { }
    public EntityNotFoundException(string message, string output) : base(message, output) { }
}
