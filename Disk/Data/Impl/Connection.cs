using Disk.Calculations.Impl.Converters;
using Disk.Data.Interface;
using DocumentFormat.OpenXml.Drawing.Diagrams;
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
    private Connection(IPAddress ip, int port, int receiveTimeout = 2000)
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

    const int Size = 48;
    int Index = Size;
    byte[] Data = new byte[Size];
    /// <inheritdoc/>
    public Point3D<float>? GetXYZ()
    {   
        if (Index >= Size)
        {
            Index = 0;
            _ = Socket.Receive(Data);
        }
        var num = BitConverter.ToInt32(Data, Index);
        var y = BitConverter.ToSingle(Data, Index + 4);
        var x = -BitConverter.ToSingle(Data, Index + 8);
        var z = BitConverter.ToSingle(Data, Index + 12);
        Index += 16;

        /*        var coordX = new byte[4];
                var coordY = new byte[4];
                var coordZ = new byte[4];
                var packetNum = new byte[4];

                _ = Socket.Receive(packetNum);
                _ = Socket.Receive(coordY);
                _ = Socket.Receive(coordX);
                _ = Socket.Receive(coordZ);

                var x = -BitConverter.ToSingle(coordX, 0);
                var y = BitConverter.ToSingle(coordY, 0);
                var z = BitConverter.ToSingle(coordZ, 0);*/

        var p = new Point3D<float>(x, y, z);

        return Converter.ToAngle_FromRadian(p);
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
