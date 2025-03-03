using System.Net;
using System.Net.Sockets;

TcpListener? server = null;
string fileName = "connection.log";
string ip = "192.168.0.105";
int port = 9998;
byte handshake = 23;

try
{
    IPAddress localAddr = IPAddress.Parse(ip);

    server = new TcpListener(localAddr, port);
    server.Start();

    TcpClient client = server.AcceptTcpClient();
    NetworkStream stream = client.GetStream();

    Console.WriteLine("Connected");

    byte[] receivedData = new byte[1];
    stream.Read(receivedData, 0, receivedData.Length);
    Console.WriteLine($"Received: {receivedData[0]}");

    byte[] data = [handshake];
    stream.Write(data, 0, data.Length);
    Console.WriteLine($"Sent: {data[0]}");

    using var reader = new StreamReader(fileName);

    while (true)
    {
        receivedData = new byte[3 * sizeof(float)];
        stream.Read(receivedData, 0, receivedData.Length);
        _ = float.TryParse(receivedData, out float value);
        Console.WriteLine(value);
    }
}
catch (Exception e)
{
    Console.WriteLine($"Error: {e.Message}");
}
finally
{
    server?.Stop();
}
