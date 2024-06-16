using System.Net;
using System.Net.Sockets;

namespace Listener
{
    /// <summary>
    ///     Represents a connection to a data source
    /// </summary>
    class Connection : IDisposable
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

        /// <summary>
        ///     Retrieves XYZ coordinates from the connection
        /// </summary>
        /// <returns>
        ///     The Point3D object representing XYZ coordinates
        /// </returns>
        public string GetXYZ()
        {
            var coordX = new byte[4];
            var coordY = new byte[4];
            var coordZ = new byte[4];

            Socket.Receive(coordX);
            Socket.Receive(coordY);
            Socket.Receive(coordZ);

            var x = -BitConverter.ToSingle(coordX, 0);
            Console.WriteLine($"Received x {x}");
            var y = BitConverter.ToSingle(coordY, 0);
            Console.WriteLine($"Received y {y}");
            var z = BitConverter.ToSingle(coordZ, 0);
            Console.WriteLine($"Received z {z}");

            return $"{x} {y} {z}";
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
