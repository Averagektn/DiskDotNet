using Listener;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

var connection = Connection.GetConnection(IPAddress.Parse("192.168.1.3"), 9998);
var stopwatch = new Stopwatch();
const int Seconds = 5 * 60;
const int Freq = 100;
var time = new List<long>(Freq * Seconds);
var avgs = new List<double>(Freq * Seconds);
var endTime = DateTime.Now.AddSeconds(Seconds);

while (DateTime.Now < endTime)
{
    stopwatch.Restart();
    var point = connection.GetXYZ();
    //Task.Delay(5).Wait();
    var ms = stopwatch.ElapsedMilliseconds;
    //Console.WriteLine($"GetXYZ: {ms}");
    time.Add(ms);
    avgs.Add(time.Average());

    //Console.WriteLine($"After add to list: {stopwatch.ElapsedMilliseconds}");
    //Console.WriteLine(point);
}

string avgsName = "avgs_buff_64.json";
string timeName = "time_buff_64.json";
var avgsTask = File.WriteAllTextAsync($"../../../../Plot/{avgsName}", JsonConvert.SerializeObject(avgs));
var avgsTaskLocal = File.WriteAllTextAsync($"./{avgsName}", JsonConvert.SerializeObject(avgs));
var timeTask = File.WriteAllTextAsync($"../../../../Plot/{timeName}", JsonConvert.SerializeObject(time));
var timeTaskLocal = File.WriteAllTextAsync($"./{timeName}", JsonConvert.SerializeObject(time));
await Task.WhenAll(timeTask, avgsTask, timeTaskLocal, avgsTaskLocal);

Console.WriteLine($"Avg: {time.Average()}");
connection.Dispose();

Console.WriteLine(time.Count);

var startInfoAvgs = new ProcessStartInfo
{
    FileName = "python",
    Arguments = $"../../../../Plot/Plot.py {avgsName}",
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
    Arguments = $"../../../../Plot/Plot.py {timeName}",
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false,
    CreateNoWindow = true
};
var processTime = Process.Start(startInfoTime);
processTime?.WaitForExit();
Console.WriteLine(processTime?.StandardError.ReadToEnd());
