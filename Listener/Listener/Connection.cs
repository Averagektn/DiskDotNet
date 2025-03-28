using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Listener;

/// <summary>
///     Represents a connection to a data source
/// </summary>
internal class Connection : IDisposable
{
    /// <summary>
    ///     The IP address of the connection
    /// </summary>
    public readonly IPAddress IP;

    /// <summary>
    ///     The port number of the connection
    /// </summary>
    public readonly int Port;

    /// <summary>
    ///     A list of active connections
    /// </summary>
    private static readonly List<Connection> Connections = [];

    /// <summary>
    ///     The underlying socket used for the connection
    /// </summary>
    private readonly Socket Socket;

    /// <summary>
    ///     Initializes a new instance of the Connection class with the specified IP address, port, and receive timeout
    /// </summary>
    /// <param name="ip">
    ///     The IP address of the connection
    /// </param>
    /// <param name="port">
    ///     The port number of the connection
    /// </param>
    /// <param name="receiveTimeout">
    ///     The receive timeout in milliseconds
    /// </param>
    private Connection(IPAddress ip, int port, int receiveTimeout = 20)
    {
        IP = ip;
        Port = port;

        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
        //Socket.ReceiveTimeout = 200; // Таймаут ожидания данных
        Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 12000);
        Socket.Connect(new IPEndPoint(IP, Port));

        Handshake();
    }

    /// <summary>
    ///     Performs the handshake protocol for the connection
    /// </summary>
    /// <exception cref="SocketException">
    ///     Thrown if the handshake fails
    /// </exception>
    private void Handshake()
    {
        byte[] receiveData = new byte[1];
        int bytesRead = Socket.Receive(receiveData);

        if (bytesRead == 1 && receiveData[0] == 23)
        {
            _ = Socket.Send(receiveData);
        }
        else
        {
            Socket.Close();
            throw new SocketException();
        }
    }

    /// <summary>
    ///     Gets a connection to the specified IP address and port. Creates a new connection if it does not exist
    /// </summary>
    /// <param name="ip">
    ///     The IP address of the connection
    /// </param>
    /// <param name="port">
    ///     The port number of the connection
    /// </param>
    /// <param name="receiveTimeout">
    ///     The receive timeout in milliseconds. Default value is 2000
    /// </param>
    /// <returns>
    ///     The Connection object
    /// </returns>
    public static Connection GetConnection(IPAddress ip, int port, int receiveTimeout = 20)
    {
        var conn = Connections.FirstOrDefault(c => c.IP.Equals(ip) && c.Port == port);

        if (conn is null)
        {
            conn = new Connection(ip, port, receiveTimeout);

            Connections.Add(conn);
        }

        return conn;
    }

    private const int PacketSize = 16;
    private const int PacketsCount = 4;
    private const int Size = PacketSize * PacketsCount;
    private int _currPacket = PacketsCount;
    private readonly byte[] _data = new byte[Size];
    private readonly List<string> _coords = ["", "", "", ""];
    /// <inheritdoc/>
    public string GetXYZ()
    {
        if (_currPacket >= PacketsCount)
        {
            _ = Socket.Receive(_data);
            for (int i = 0, j = 0; i < Size; i += PacketSize, j++)
            {
                var y = BitConverter.ToSingle(_data, i + 4);
                var x = -BitConverter.ToSingle(_data, i + 8);
                var z = BitConverter.ToSingle(_data, i + 12);

                //_coords[j] = Converter.ToAngle_FromRadian(new Point3D<float>(x, y, z));
                _coords[j] = $"{x} {y} {x}";
            }
            _currPacket = 0;
        }
        else if (Socket.Available < Size * 2)
        {
            Task.Delay(1).Wait();
            //Thread.Sleep(2);
        }

        return _coords[_currPacket++];
    
        //return $"{x} {y} {z} {num}";
        /*        var coordX = new byte[4];
                var coordY = new byte[4];
                var coordZ = new byte[4];
                var packetNum = new byte[4];
                _stopwatch.Restart();

                _ = Socket.Receive(packetNum);
                var numTime = _stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"Num: {numTime}");

                _ = Socket.Receive(coordY);
                var yTime = _stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"Y: {yTime - numTime}ms");

                _ = Socket.Receive(coordX);
                var xTime = _stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"X: {xTime - yTime}ms");

                _ = Socket.Receive(coordZ);
                var zTime = _stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"Z: {zTime - xTime}ms");

                var num = BitConverter.ToInt32(packetNum, 0);
                Console.WriteLine($"Received packet: {num}");

                var y = BitConverter.ToSingle(coordY, 0);
                Console.WriteLine($"Received y {y}");

                var x = -BitConverter.ToSingle(coordX, 0);
                Console.WriteLine($"Received x {x}");

                var z = BitConverter.ToSingle(coordZ, 0);
                Console.WriteLine($"Received z {z}");

                return $"{x} {y} {z}";*/
    }

    /// <summary>
    ///     Disposes the connection and removes it from the list of active connections
    /// </summary>
    public void Dispose()
    {
        _ = Connections.Remove(this);
        Socket.Close();
    }
}
