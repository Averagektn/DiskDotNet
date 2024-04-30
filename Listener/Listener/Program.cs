using Listener;
using System.Diagnostics;
using System.Net;

var connection = Connection.GetConnection(IPAddress.Parse("192.168.150.4"), 9998);
var stopwatch = new Stopwatch();

while (true)
{
    stopwatch.Start();
    var str = connection.GetXYZ();
    stopwatch.Stop();
    Console.WriteLine($"{str} timeout {stopwatch.ElapsedMilliseconds}ms");
    stopwatch.Reset();
}
