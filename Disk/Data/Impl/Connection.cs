﻿using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Media.Media3D;

namespace Disk.Data
{
    class Connection : IDataSourceF, IDisposable
    {
        private readonly Logger Log;

        private static readonly List<Connection> Connections = [];

        private readonly Socket Socket;

        public IPAddress IP { get; private set; }

        public int Port { get; private set; }

        private Connection(IPAddress ip, int port)
        {
            Log = Logger.GetLogger("Connection/Connection.log", ' ');
            IP = ip;
            Port = port;
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(new IPEndPoint(IP, Port));
            Handshake();
        }

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

        public static Connection GetConnection(IPAddress ip, int port, object handshake)
        {
            var conn = Connections.FirstOrDefault(c => c.IP.Equals(ip) && c.Port == port);

            if (conn is null)
            {
                conn = new(ip, port);

                Connections.Add(conn);
            }

            return conn;
        }

        public static void CloseConnection(IPAddress ip, int port)
        {
            var conn = Connections.FirstOrDefault(c => c.IP.Equals(ip) && c.Port == port);

            conn?.Dispose();
        }

        public Point3D GetXYZ()
        {
            var coords = new byte[12];
            Socket.Receive(coords);

            float x = BitConverter.ToSingle(coords, 0);
            float y = BitConverter.ToSingle(coords, 4);
            float z = BitConverter.ToSingle(coords, 8);

            return new(x, y, z);
        }

        public PointF GetXY()
        {
            var data = GetXYZ();

            return new((float)data.X, (float)data.Y);
        }

        public PointF GetXZ()
        {
            var data = GetXYZ();

            return new((float)data.X, (float)data.Z);
        }

        public PointF GetYX()
        {
            var data = GetXYZ();

            return new((float)data.Y, (float)data.X);
        }

        public PointF GetYZ()
        {
            var data = GetXYZ();

            return new((float)data.Y, (float)data.Z);
        }

        public PointF GetZX()
        {
            var data = GetXYZ();

            return new((float)data.Z, (float)data.X);
        }

        public PointF GetZY()
        {
            var data = GetXYZ();

            return new((float)data.Z, (float)data.Y);
        }

        public void Dispose()
        {
            Socket.Close();
        }
    }
}
