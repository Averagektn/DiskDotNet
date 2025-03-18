namespace Disk.Properties.Config;

public static class AppConfig
{
    public const string DbConnectionString = @"DataSource=.\Db\disk.db";
    public const string DbPath = DbDir + DbFileName;
    public const string DbFileName = @"disk.db";
    public const string DbDir = @".\Db\";
}
