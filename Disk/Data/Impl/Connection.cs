using Disk.Calculations.Implementations.Converters;
using Disk.Data.Interface;
using System.Buffers.Binary;
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
            ReceiveTimeout = receiveTimeout,
            //NoDelay = true,
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
    private const int PacketsCount = 5;
    private const int Size = PacketSize * PacketsCount;
    private int _currPacket = PacketsCount;
    private readonly byte[] _data = new byte[Size];
    private readonly float[] _coords = new float[PacketsCount * 2];
    /// <inheritdoc/>
    public Point3D<float>? GetXYZ()
    {
        if (_currPacket >= PacketsCount)
        {
            int received = 0;
            while (received < Size)
            {
                received += Socket.Receive(_data, received, Size - received, SocketFlags.None);
            }

            for (int j = 0, offset = 0; j < PacketsCount; j++, offset += PacketSize)
            {
                float y = BinaryPrimitives.ReadSingleLittleEndian(_data.AsSpan(offset + 4, 4));
                float x = -BinaryPrimitives.ReadSingleLittleEndian(_data.AsSpan(offset + 8, 4));

                _coords[j * 2] = Converter.ToAngle_FromRadian(x);
                _coords[(j * 2) + 1] = Converter.ToAngle_FromRadian(y);

                //_coords[j] = new Point3D<float>(angleX, angleY, 0);
            }
            _currPacket = 0;
        }
        //else if (Socket.Available < Size * 2)
        else
        {
            Task.Delay(1).Wait();
        }

        var res = new Point3D<float>(_coords[_currPacket * 2], _coords[(_currPacket * 2) + 1], 0);
        _currPacket++;

        return res;
    }

/*    public Point3D<float>? GetXYZ()
    {
        if (_currPacket >= PacketsCount)
        {
            _ = Socket.Receive(_data, Size, SocketFlags.None);
            var span = new ReadOnlySpan<byte>(_data);

            for (int i = 0, j = 0; i < Size; i += PacketSize, j++)
            {
                var y = BinaryPrimitives.ReadSingleLittleEndian(span[(i + 4)..]);
                var x = -BinaryPrimitives.ReadSingleLittleEndian(span[(i + 8)..]);

                var angleX = Converter.ToAngle_FromRadian(x);
                var angleY = Converter.ToAngle_FromRadian(y);

                _coords[j] = new Point3D<float>(angleX, angleY, 0);
            }
            _currPacket = 0;
        }
        else if (Socket.Available < Size * 2)
        {
            Task.Delay(1).Wait();
        }

        return _coords[_currPacket++];
    }*/

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
