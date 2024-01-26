using Disk.Calculations.Impl;
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
        private Connection(IPAddress ip, int port, int receiveTimeout)
        {
            Logger = Logger.GetLogger("connection.log");

            IP = ip;
            Port = port;

            Socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = receiveTimeout
            };
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
        public static Connection GetConnection(IPAddress ip, int port, int receiveTimeout = 2000)
        {
            var conn = Connections.FirstOrDefault(c => c.IP.Equals(ip) && c.Port == port);

            if (conn is null)
            {
                conn = new(ip, port, receiveTimeout);

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
        public Point3D<float>? GetXYZ()
        {
            var coordX = new byte[4];
            var coordY = new byte[4];
            var coordZ = new byte[4];

            Socket.Receive(coordX);
            Socket.Receive(coordY);
            Socket.Receive(coordZ);

            var x = BitConverter.ToSingle(coordX, 0);
            var y = BitConverter.ToSingle(coordY, 0);
            var z = BitConverter.ToSingle(coordZ, 0);

            var p = new Point3D<float>(x, y, z);

            Logger.LogLn(p);

            return Converter.ToAngle_FromRadian(p);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<float>? GetXY()
        {
            var data = GetXYZ();

            return data is null ? null : new((float)data.X, (float)data.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<float>? GetXZ()
        {
            var data = GetXYZ();

            return data is null ? null : new((float)data.X, (float)data.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<float>? GetYX()
        {
            var data = GetXYZ();

            return data is null ? null : new((float)data.Y, (float)data.X);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<float>? GetYZ()
        {
            var data = GetXYZ();

            return data is null ? null : new((float)data.Y, (float)data.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<float>? GetZX()
        {
            var data = GetXYZ();

            return data is null ? null : new((float)data.Z, (float)data.X);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<float>? GetZY()
        {
            var data = GetXYZ();

            return data is null ? null : new((float)data.Z, (float)data.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Connections.Remove(this);

            Logger.Dispose();

            Socket.Close();
        }
    }
}
