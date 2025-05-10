using Disk.Services.Exceptions.Common;

namespace Disk.Services.Exceptions;

public class InvalidHomePhoneException : ServiceException
{
    public InvalidHomePhoneException(string message) : base(message) { }
    public InvalidHomePhoneException(string message, string output) : base(message, output) { }
}
