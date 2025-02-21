using Disk.Service.Exceptions.Common;

namespace Disk.Service.Exceptions;

public class InvalidDateException : ServiceException
{
    public InvalidDateException(string message) : base(message) { }
    public InvalidDateException(string message, string output) : base(message, output) { }
}
