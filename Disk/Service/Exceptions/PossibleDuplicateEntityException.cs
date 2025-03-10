using Disk.Service.Exceptions.Common;

namespace Disk.Service.Exceptions;

public class PossibleDuplicateEntityException : ServiceException
{
    public PossibleDuplicateEntityException(string message) : base(message) { }
    public PossibleDuplicateEntityException(string message, string output) : base(message, output) { }
}
