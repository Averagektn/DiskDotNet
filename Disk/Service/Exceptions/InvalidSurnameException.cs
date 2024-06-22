using Disk.Service.Exceptions.Common;

namespace Disk.Service.Exceptions
{
    public class InvalidSurnameException : ServiceException
    {
        public InvalidSurnameException(string message) : base(message) { }
        public InvalidSurnameException(string output, string message) : base(output, message) { }
    }
}
