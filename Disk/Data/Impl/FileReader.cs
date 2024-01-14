using System.Drawing;

namespace Disk.Data
{
    // make enumerable
    class FileReader : IDataSource
    {
        public FileReader(string filename)
        {

        }

        public IEnumerator<Point> GetEnumerator_Point()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<PointF> GetEnumerator_PointF()
        {
            throw new NotImplementedException();
        }

        public string ReadLn()
        {
            throw new NotImplementedException();
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
