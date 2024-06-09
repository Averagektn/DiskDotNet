using Disk.Service.Exceptions.Common;

namespace Disk.Service.Exceptions
{
    public class InvalidPasswordException : ServiceException 
    {
        public InvalidPasswordException(string message) : base(message) { }
        public InvalidPasswordException(string output, string message) : base(output, message) { }
    }
}
