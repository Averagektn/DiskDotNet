using Disk.Calculations.Impl.Converters;
using Disk.Data.Interface;
using System.Net;
using System.Net.Sockets;

namespace Disk.Data.Impl
{
    /// <summary>
    ///     Represents a connection to a data source
    /// </summary>
    class Connection : IDataSource<float>, IDisposable
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
        private Connection(IPAddress ip, int port, int receiveTimeout)
        {
            IP = ip;
            Port = port;

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
/*            {
                ReceiveTimeout = receiveTimeout
            }*/;
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
                Socket.Send(receiveData);
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

            if (conn is null)
            {
                conn = new Connection(ip, port, receiveTimeout);

                Connections.Add(conn);
            }

            return conn;
        }

        /// <summary>
        ///     Retrieves XYZ coordinates from the connection
        /// </summary>
        /// <returns>
        ///     The Point3D object representing XYZ coordinates
        /// </returns>
        public Point3D<float>? GetXYZ()
        {
            var coordX = new byte[4];
            var coordY = new byte[4];
            var coordZ = new byte[4];

            Socket.Receive(coordX);
            Socket.Receive(coordY);
            Socket.Receive(coordZ);

            var x = -BitConverter.ToSingle(coordX, 0);
            var y = BitConverter.ToSingle(coordY, 0);
            var z = BitConverter.ToSingle(coordZ, 0);

            var p = new Point3D<float>(x, y, z);

            return Converter.ToAngle_FromRadian(p);
        }

        /// <summary>
        ///     Retrieves XY coordinates from the connection
        /// </summary>
        /// <returns>
        ///     The Point2D object representing XY coordinates
        /// </returns>
        public Point2D<float>? GetXY()
        {
            var data = GetXYZ();

            return data is null ? null : new Point2D<float>((float)data.X, (float)data.Y);
        }

        /// <summary>
        ///     Retrieves XZ coordinates from the connection
        /// </summary>
        /// <returns>
        ///     The Point2D object representing XZ coordinates
        /// </returns>
        public Point2D<float>? GetXZ()
        {
            var data = GetXYZ();

            return data is null ? null : new Point2D<float>((float)data.X, (float)data.Z);
        }

        /// <summary>
        ///     Retrieves YX coordinates from the connection
        /// </summary>
        /// <returns>
        ///     The Point2D object representing YX coordinates
        /// </returns>
        public Point2D<float>? GetYX()
        {
            var data = GetXYZ();

            return data is null ? null : new Point2D<float>((float)data.Y, (float)data.X);
        }

        /// <summary>
        ///     Retrieves YZ coordinates from the connection
        /// </summary>
        /// <returns>
        ///     The Point2D object representing YZ coordinates
        /// </returns>
        public Point2D<float>? GetYZ()
        {
            var data = GetXYZ();

            return data is null ? null : new Point2D<float>((float)data.Y, (float)data.Z);
        }

        /// <summary>
        ///     Retrieves ZX coordinates from the connection
        /// </summary>
        /// <returns>
        ///     The Point2D object representing ZX coordinates
        /// </returns>
        public Point2D<float>? GetZX()
        {
            var data = GetXYZ();

            return data is null ? null : new Point2D<float>((float)data.Z, (float)data.X);
        }

        /// <summary>
        ///     Retrieves ZY coordinates from the connection
        /// </summary>
        /// <returns>
        ///     The Point2D object representing ZY coordinates
        /// </returns>
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
            Connections.Remove(this);
            Socket.Close();
        }
    }
}
