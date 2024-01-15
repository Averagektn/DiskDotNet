using System.Drawing;
using System.Net;
using System.Net.Sockets;

namespace Disk.Data
{
    class Connection : IDataSource, IDisposable
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

        public Point GetXY()
        {
            throw new NotImplementedException();
        }

        public Point GetXZ()
        {
            throw new NotImplementedException();
        }

        public Point GetYX()
        {
            throw new NotImplementedException();
        }

        public Point GetYZ()
        {
            throw new NotImplementedException();
        }

        public Point GetZX()
        {
            throw new NotImplementedException();
        }

        public Point GetZY()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Socket.Close();
        }
    }
}
