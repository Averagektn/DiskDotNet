using Disk.Services.Exceptions.Common;

namespace Disk.Services.Exceptions;

public class InvalidPhoneNumberException : ServiceException
{
    public InvalidPhoneNumberException(string message) : base(message) { }
    public InvalidPhoneNumberException(string message, string output) : base(message, output) { }
}
