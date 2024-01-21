using Disk.Data.Interface;
using System.Net;
using System.Net.Sockets;

namespace Disk.Data.Impl
{
    class Connection : IDataSource<float>, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly IPAddress IP;

        /// <summary>
        /// 
        /// </summary>
        public readonly int Port;

        private static readonly List<Connection> Connections = [];

        private readonly Logger Logger;
        private readonly Socket Socket;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip">
        /// 
        /// </param>
        /// <param name="port">
        /// 
        /// </param>
        private Connection(IPAddress ip, int port)
        {
            Logger = Logger.GetLogger("connection.log");

            IP = ip;
            Port = port;

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(new IPEndPoint(IP, Port));

            Handshake();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="SocketException">
        /// 
        /// </exception>
        private void Handshake()
        {
            byte[] receiveData = new byte[1];
            int bytesRead = Socket.Receive(receiveData);

            if (bytesRead == 1 && receiveData[0] == 0x23)
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
        /// 
        /// </summary>
        /// <param name="ip">
        /// 
        /// </param>
        /// <param name="port">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Connection GetConnection(IPAddress ip, int port)
        {
            var conn = Connections.FirstOrDefault(c => c.IP.Equals(ip) && c.Port == port);

            if (conn is null)
            {
                conn = new(ip, port);

                Connections.Add(conn);
            }

            return conn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point3D<float> GetXYZ()
        {
            var coords = new byte[12];

            Socket.Receive(coords);

            float x = BitConverter.ToSingle(coords, 0);
            float y = BitConverter.ToSingle(coords, 4);
            float z = BitConverter.ToSingle(coords, 8);

            var p = new Point3D<float>(x, y, z);

            Logger.LogLn(p);

            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<float> GetXY()
        {
            var data = GetXYZ();

            return new((float)data.X, (float)data.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<float> GetXZ()
        {
            var data = GetXYZ();

            return new((float)data.X, (float)data.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<float> GetYX()
        {
            var data = GetXYZ();

            return new((float)data.Y, (float)data.X);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<float> GetYZ()
        {
            var data = GetXYZ();

            return new((float)data.Y, (float)data.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<float> GetZX()
        {
            var data = GetXYZ();

            return new((float)data.Z, (float)data.X);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<float> GetZY()
        {
            var data = GetXYZ();

            return new((float)data.Z, (float)data.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Connections.Remove(this);

            Socket.Close();
        }
    }
}
