using Disk.Services.Exceptions.Common;

namespace Disk.Services.Exceptions;

public class DuplicateEntityException : ServiceException
{
    public DuplicateEntityException(string message) : base(message) { }
    public DuplicateEntityException(string message, string output) : base(message, output) { }
}
