namespace Disk.Exceptions
{
    public class BaseException : Exception
    {
        public string Output { get; protected set; } = string.Empty;

        public BaseException(string message) : base(message) { }

        public BaseException(string output, string message) : base(message)
        {
            Output = output;
        }
    }
}
