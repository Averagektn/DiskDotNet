using Disk.Service.Exceptions.Common;

namespace Disk.Service.Exceptions
{
    public class InvalidPhoneNumberException : ServiceException
    {
        public InvalidPhoneNumberException(string message) : base(message) { }
        public InvalidPhoneNumberException(string output, string message) : base(output, message) { }
    }
}
