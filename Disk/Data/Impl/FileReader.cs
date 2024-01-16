using Disk.Data.Interface;
using System.Collections;

namespace Disk.Data
{
    // idatasource
    // ienumerable
    class FileReader<PointType3D, PointType2D, CoordType> :
        IEnumerable<PointType3D>,
        IDataSource<PointType3D, PointType2D, CoordType>
        where PointType3D :
            IPoint<CoordType>
        where PointType2D :
            IPoint<CoordType>
        where CoordType :
            IComparable,
            IFormattable,
            IConvertible,
            IComparable<CoordType>,
            IEquatable<CoordType>
    {
        private static readonly List<FileReader<PointType3D, PointType2D, CoordType>> Files = [];

        public readonly string Filename;

        public readonly char Separator;

        private FileReader(string filename, char separator)
        {
            Filename = filename;
            Separator = separator;
        }

        public static FileReader<PointType3D, PointType2D, CoordType> Open(string filename, char separator)
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

        public string ReadLn()
        {
            throw new NotImplementedException();
        }

        public PointType3D GetXYZ()
        {
            throw new NotImplementedException();
        }

        public PointType2D GetXY()
        {
            throw new NotImplementedException();
        }

        public PointType2D GetYZ()
        {
            throw new NotImplementedException();
        }

        public PointType2D GetXZ()
        {
            throw new NotImplementedException();
        }

        public PointType2D GetYX()
        {
            throw new NotImplementedException();
        }

        public PointType2D GetZY()
        {
            throw new NotImplementedException();
        }

        public PointType2D GetZX()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<PointType3D> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
