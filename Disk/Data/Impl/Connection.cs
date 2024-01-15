using System.Drawing;
using System.Net;

namespace Disk.Data
{
    class Connection : IDataSource
    {
        private static List<Connection> Connections = [];
        public IPAddress IP;
        public int Port;
        private Connection(IPAddress ip, int port, object handshake)
        {
            IP = ip;
            Port = port;
            // Establish connection
        }

        public static Connection GetConnection(IPAddress ip, int port, object handshake)
        {
            var conn = Connections.FirstOrDefault(c => c.IP.Equals(ip) && c.Port == port);

            if (conn is null)
            {
                conn = new(ip, port, handshake);

                Connections.Add(conn);
            }

            return conn;
        }

        public static void CloseConnection(IPAddress ip, int port)
        {
            var conn = Connections.FirstOrDefault(c => c.IP.Equals(ip) && c.Port == port);

            conn?.Close();
        }

        private void Close()
        {

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
    }
}
