using Listener;
using System.Diagnostics;
using System.Net;

var connection = Connection.GetConnection(IPAddress.Parse("192.168.150.2"), 9998);
var stopwatch = new Stopwatch();

while (true)
{
    stopwatch.Start();
    var str = connection.GetXYZ();
    stopwatch.Stop();
    var ms = stopwatch.ElapsedMilliseconds;
    Console.ForegroundColor = ms switch
    {
        < 10 => ConsoleColor.Green,
        >= 10 and < 50 => ConsoleColor.Yellow,
        >= 50 and < 100 => ConsoleColor.Red,
        >= 100 => ConsoleColor.DarkRed
    };

    Console.WriteLine($"{str} timeout {stopwatch.ElapsedMilliseconds}ms");

    Console.ResetColor();
    stopwatch.Reset();
}
