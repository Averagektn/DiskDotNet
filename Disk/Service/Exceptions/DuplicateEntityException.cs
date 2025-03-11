using Disk.Service.Exceptions.Common;

namespace Disk.Service.Exceptions;

public class DuplicateEntityException : ServiceException
{
    public DuplicateEntityException(string message) : base(message) { }
    public DuplicateEntityException(string message, string output) : base(message, output) { }
}
