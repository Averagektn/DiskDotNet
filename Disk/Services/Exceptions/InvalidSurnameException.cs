using Disk.Services.Exceptions.Common;

namespace Disk.Services.Exceptions;

public class InvalidSurnameException : ServiceException
{
    public InvalidSurnameException(string message) : base(message) { }
    public InvalidSurnameException(string message, string output) : base(message, output) { }
}
