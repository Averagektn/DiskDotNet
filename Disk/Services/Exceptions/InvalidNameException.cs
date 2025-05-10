using Disk.Services.Exceptions.Common;

namespace Disk.Services.Exceptions;

public class InvalidNameException : ServiceException
{
    public InvalidNameException(string message) : base(message) { }
    public InvalidNameException(string message, string output) : base(message, output) { }
}
