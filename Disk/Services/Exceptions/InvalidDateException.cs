using Disk.Services.Exceptions.Common;

namespace Disk.Services.Exceptions;

public class InvalidDateException : ServiceException
{
    public InvalidDateException(string message) : base(message) { }
    public InvalidDateException(string message, string output) : base(message, output) { }
}
