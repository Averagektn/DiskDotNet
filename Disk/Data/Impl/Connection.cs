using Disk.Calculations.Impl.Converters;
using Disk.Data.Interface;
using System.Net;
using System.Net.Sockets;

namespace Disk.Data.Impl;

/// <summary>
///     Represents a connection to a data source
/// </summary>
public class Connection : IDataSource<float>, IDisposable
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
    ///     Lock object
    /// </summary>
    private static readonly Lock _lockObj = new();

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
    private Connection(IPAddress ip, int port, int receiveTimeout = 5000)
    {
        IP = ip;
        Port = port;

        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            ReceiveTimeout = receiveTimeout
        };
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
    public static Connection GetConnection(IPAddress ip, int port, int receiveTimeout = 2000)
    {
        var conn = Connections.FirstOrDefault(c => c.IP.Equals(ip) && c.Port == port);

        lock (_lockObj)
        {
            if (conn is null)
            {
                conn = new Connection(ip, port, receiveTimeout);

                Connections.Add(conn);
            }
        }

        return conn;
    }

    private const int PacketSize = 16;
    private const int PacketsCount = 4;
    private const int Size = PacketSize * PacketsCount;
    private int _currPacket = PacketsCount;
    private readonly byte[] _data = new byte[Size];
    private readonly List<Point3D<float>> _coords = [new(), new(), new(), new()];
    /// <inheritdoc/>
    public Point3D<float>? GetXYZ()
    {
        if (_currPacket >= PacketsCount)
        {
            _ = Socket.Receive(_data, Size, SocketFlags.None);
            for (int i = 0, j = 0; i < Size; i += PacketSize, j++)
            {
                var y = BitConverter.ToSingle(_data, i + 4);
                var x = -BitConverter.ToSingle(_data, i + 8);
                var z = BitConverter.ToSingle(_data, i + 12);

                _coords[j] = Converter.ToAngle_FromRadian(new Point3D<float>(x, y, z));
            }
            _currPacket = 0;
        }
        else if (Socket.Available < Size * 2)
        {
            Task.Delay(1).Wait();
            //Thread.Sleep(2);
        }

        return _coords[_currPacket++];
    }

    /// <inheritdoc/>
    public Point2D<float>? GetXY()
    {
        var data = GetXYZ();

        return data is null ? null : new Point2D<float>((float)data.X, (float)data.Y);
    }

    /// <inheritdoc/>
    public Point2D<float>? GetXZ()
    {
        var data = GetXYZ();

        return data is null ? null : new Point2D<float>((float)data.X, (float)data.Z);
    }

    /// <inheritdoc/>
    public Point2D<float>? GetYX()
    {
        var data = GetXYZ();

        return data is null ? null : new Point2D<float>((float)data.Y, (float)data.X);
    }

    /// <inheritdoc/>
    public Point2D<float>? GetYZ()
    {
        var data = GetXYZ();

        return data is null ? null : new Point2D<float>((float)data.Y, (float)data.Z);
    }

    /// <inheritdoc/>
    public Point2D<float>? GetZX()
    {
        var data = GetXYZ();

        return data is null ? null : new Point2D<float>((float)data.Z, (float)data.X);
    }

    /// <inheritdoc/>
    public Point2D<float>? GetZY()
    {
        var data = GetXYZ();

        return data is null ? null : new Point2D<float>((float)data.Z, (float)data.Y);
    }

    /// <summary>
    ///     Disposes the connection and removes it from the list of active connections
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _ = Connections.Remove(this);
        Socket.Close();
    }
}
