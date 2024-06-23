namespace Disk.Extensions
{
    public static class StringExtensions
    {
        public static string CapitalizeFirstLetter(this string str) => string.IsNullOrEmpty(str) ? str : char.ToUpper(str[0]) + str[1..];
    }
}
