using Disk.Data.Interface;
using System.Windows.Media.Media3D;

namespace Disk.Data
{
    class FileReader<PointType> : IDataSource<Point3D, PointType>
    {
        private static readonly List<FileReader<PointType>> Files = [];

        public readonly string Filename;

        public readonly string Separator;

        private FileReader(string filename, string separator)
        {
            Filename = filename;
            Separator = separator;
        }

        public static FileReader<PointType> Open(string filename, string separator)
        {
            var reader = Files.FirstOrDefault(f => f.Filename == filename);

            if (reader is null)
            {
                reader = new(filename, separator);
                Files.Add(reader);
            }

            return reader;
        }

        public static void Close(string filename)
        {

        }

        public IEnumerator<Point3D> GetEnumerator_Point3D()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<PointType> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public string ReadLn()
        {
            throw new NotImplementedException();
        }

        public Point3D GetXYZ()
        {
            throw new NotImplementedException();
        }

        public PointType GetXY()
        {
            throw new NotImplementedException();
        }

        public PointType GetYZ()
        {
            throw new NotImplementedException();
        }

        public PointType GetXZ()
        {
            throw new NotImplementedException();
        }

        public PointType GetYX()
        {
            throw new NotImplementedException();
        }

        public PointType GetZY()
        {
            throw new NotImplementedException();
        }

        public PointType GetZX()
        {
            throw new NotImplementedException();
        }
    }
}
