﻿using Disk.Data.Interface;
using System.Net;
using System.Net.Sockets;

namespace Disk.Data.Impl
{
    class Connection : IDataSource<Point3D<float>, Point2D<float>, float>, IDisposable
    {
        public IPAddress IP { get; private set; }

        public int Port { get; private set; }

        private readonly Logger<Point3D<float>> Log3D;

        private static readonly List<Connection> Connections = [];

        private readonly Socket Socket;

        private Connection(IPAddress ip, int port)
        {
            Log3D = Logger<Point3D<float>>.GetLogger("Connection/Connection.log");

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

        // log
        public Point3D<float> GetXYZ()
        {
            var coords = new byte[12];
            Socket.Receive(coords);

            float x = BitConverter.ToSingle(coords, 0);
            float y = BitConverter.ToSingle(coords, 4);
            float z = BitConverter.ToSingle(coords, 8);

            return new(x, y, z);
        }

        public Point2D<float> GetXY()
        {
            var data = GetXYZ();

            return new((float)data.X, (float)data.Y);
        }

        public Point2D<float> GetXZ()
        {
            var data = GetXYZ();

            return new((float)data.X, (float)data.Z);
        }

        public Point2D<float> GetYX()
        {
            var data = GetXYZ();

            return new((float)data.Y, (float)data.X);
        }

        public Point2D<float> GetYZ()
        {
            var data = GetXYZ();

            return new((float)data.Y, (float)data.Z);
        }

        public Point2D<float> GetZX()
        {
            var data = GetXYZ();

            return new((float)data.Z, (float)data.X);
        }

        public Point2D<float> GetZY()
        {
            var data = GetXYZ();

            return new((float)data.Z, (float)data.Y);
        }

        // remove from list
        public void Dispose()
        {
            Socket.Close();
        }
    }
}
