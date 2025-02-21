using Disk.Service.Exceptions.Common;

namespace Disk.Service.Exceptions;

public class InvalidHomePhoneException : ServiceException
{
    public InvalidHomePhoneException(string message) : base(message) { }
    public InvalidHomePhoneException(string message, string output) : base(message, output) { }
}
