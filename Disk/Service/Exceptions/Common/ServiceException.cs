namespace Disk.Service.Exceptions.Common
{
    public class ServiceException : Exception
    {
        public string Output { get; protected set; } = string.Empty;

        public ServiceException(string message) : base(message) { }

        public ServiceException(string output, string message) : base(message) 
        {
            Output = output;
        }
    }
}
