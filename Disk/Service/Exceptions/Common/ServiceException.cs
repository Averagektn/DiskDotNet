using Disk.Exceptions;

namespace Disk.Service.Exceptions.Common;

public class ServiceException : BaseException
{
    public ServiceException(string message) : base(message) { }
    public ServiceException(string message, string output) : base(message, output) { }
}
