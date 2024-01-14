using System.ComponentModel;
using System.Drawing;
using System.Net;

namespace Disk.Data
{
    class Connection : IDataSource
    {
        private static List<Connection> connnections = [];
        private IPAddress IP;
        private int Port;
        private Connection(IPAddress ip, int port)
        {
            IP = ip;
            Port = port;
            // Establish connection
        }

        public static Connection GetConnection(IPAddress ip, int port)
        {
            var conn = connnections.FirstOrDefault(c => c.IP.Equals(ip) && c.Port == port);

            if (conn is null)
            {
                var c = new Connection(ip, port);   
                connnections.Add(c);
                return c;
            }

            return conn;
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
