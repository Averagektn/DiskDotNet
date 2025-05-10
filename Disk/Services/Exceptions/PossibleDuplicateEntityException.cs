using Disk.Services.Exceptions.Common;

namespace Disk.Services.Exceptions;

public class PossibleDuplicateEntityException : ServiceException
{
    public PossibleDuplicateEntityException(string message) : base(message) { }
    public PossibleDuplicateEntityException(string message, string output) : base(message, output) { }
}
