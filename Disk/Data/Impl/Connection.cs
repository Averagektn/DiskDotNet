using System.Drawing;
using System.Net;

namespace Disk.Data
{
    class Connection : IDataSource
    {
        private static List<Connection> Connnections = [];
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
            var conn = Connnections.FirstOrDefault(c => c.IP.Equals(ip) && c.Port == port);

            if (conn is null)
            {
                var c = new Connection(ip, port, handshake);

                Connnections.Add(c);

                return c;
            }

            return conn;
        }

        public static void CloseConnection(IPAddress ip, int port)
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
