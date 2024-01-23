using System.Net;
using System.Net.Sockets;

TcpListener? server = null;
string fileName = "dataset_12_working.txt";
string ip = "127.0.0.1";
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

    byte[] data = [handshake];
    stream.Write(data, 0, data.Length);
    Console.WriteLine($"Sent: {data[0]}");

    byte[] receivedData = new byte[1];
    stream.Read(receivedData, 0, receivedData.Length);
    Console.WriteLine($"Received: {receivedData[0]}");

    using var reader = new StreamReader(fileName);
    string? line;

    while ((line = reader.ReadLine()) != null)
    {
        line = line.Replace('.', ',');

        Console.WriteLine(line);
        string[] numbers = line.Split(' ');

        if (numbers.Length < 3)
        {
            Console.WriteLine("Incorrect string format");
            continue;
        }

        byte[] floatBytes = new byte[3 * sizeof(float)];

        for (int i = 0; i < 3; i++)
        {
            if (!float.TryParse(numbers[i], out float value))
            {
                Console.WriteLine("Incorrect number format");
                continue;
            }

            byte[] bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, floatBytes, i * sizeof(float), sizeof(float));
        }

        stream.Write(floatBytes, 0, floatBytes.Length);
    }

    while (true) { }
}
catch (Exception e)
{
    Console.WriteLine($"Error: {e.Message}");
}
finally
{
    server?.Stop();
}
