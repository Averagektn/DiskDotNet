namespace Disk.Data.Interface
{
    interface ILogger<DataType>
    {
        void Log(DataType data);

        void LogLn(DataType data);
    }
}
