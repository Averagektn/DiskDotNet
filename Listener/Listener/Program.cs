using Listener;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

var connection = Connection.GetConnection(IPAddress.Parse("192.168.1.3"), 9998);
var stopwatch = new Stopwatch();
const int Seconds = 1 * 60;
const int Freq = 60;
var time = new List<long>(Freq * Seconds);
var avgs = new List<double>(Freq * Seconds);
var endTime = DateTime.Now.AddSeconds(Seconds);

while (DateTime.Now < endTime)
{
    stopwatch.Restart();
    var point = connection.GetXYZ();
    var ms = stopwatch.ElapsedMilliseconds;

    time.Add(ms);
    avgs.Add(time.Average());

    Console.WriteLine($"After add to list: {stopwatch.ElapsedMilliseconds}");
    Console.WriteLine(point);
}

var avgsTask = File.WriteAllTextAsync("../../../../Plot/avgs.json", JsonConvert.SerializeObject(avgs));
var timeTask = File.WriteAllTextAsync("../../../../Plot/time.json", JsonConvert.SerializeObject(time));
await avgsTask;
await timeTask;

Console.WriteLine($"Avg: {time.Average()}");
connection.Dispose();

Console.WriteLine(time.Count);

var startInfoAvgs = new ProcessStartInfo
{
    FileName = "python",
    Arguments = $"../../../../Plot/Plot.py avgs.json",
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false,
    CreateNoWindow = true
};
var processAvgs = Process.Start(startInfoAvgs);
processAvgs?.WaitForExit();
Console.WriteLine(processAvgs?.StandardError.ReadToEnd());

var startInfoTime = new ProcessStartInfo
{
    FileName = "python",
    Arguments = $"../../../../Plot/Plot.py time.json",
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false,
    CreateNoWindow = true
};
var processTime = Process.Start(startInfoTime);
processTime?.WaitForExit();
Console.WriteLine(processTime?.StandardError.ReadToEnd());
