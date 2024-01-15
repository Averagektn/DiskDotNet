using Disk.Data.Interface;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace Disk.Data
{
    // make enumerable
    class FileReader : IDataSourceF, IDataSource
    {
        private static List<FileReader> Files = [];
        public readonly string Filename;

        private FileReader(string filename)
        {
            Filename = filename;
        }

        public static FileReader Open(string filename)
        {
            var reader = Files.FirstOrDefault(f => f.Filename == filename);

            if (reader is null)
            {
                reader = new(filename);
                Files.Add(reader);    
            }

            return reader;
        }

        public static void Close(string filename)
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

        PointF IDataSourceF.GetXY()
        {
            throw new NotImplementedException();
        }

        PointF IDataSourceF.GetYZ()
        {
            throw new NotImplementedException();
        }

        PointF IDataSourceF.GetXZ()
        {
            throw new NotImplementedException();
        }

        PointF IDataSourceF.GetYX()
        {
            throw new NotImplementedException();
        }

        PointF IDataSourceF.GetZY()
        {
            throw new NotImplementedException();
        }

        PointF IDataSourceF.GetZX()
        {
            throw new NotImplementedException();
        }

        public Point3D GetXYZ()
        {
            throw new NotImplementedException();
        }
    }
}
