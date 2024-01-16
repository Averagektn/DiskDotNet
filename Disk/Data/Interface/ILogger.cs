namespace Disk.Data
{
    interface ILogger<DataType> 
    {
        void Log(DataType data);

        void LogLn(DataType data);
    }
}
