using Disk.Data.Impl;
using Disk.Data.Interface;
using System.Collections;
using System.IO;

namespace Disk.Data
{
    class FileReader<PointType3D, PointType2D, CoordType> :
        IEnumerable<PointType3D>,
        IDataSource<PointType3D, PointType2D, CoordType>,
        IDisposable
        where PointType3D :
            Point3D<CoordType>,
            new()
        where PointType2D :
            Point2D<CoordType>,
            new()
        where CoordType :
            new()
    {
        private static readonly List<FileReader<PointType3D, PointType2D, CoordType>> Files = [];

        public readonly string Filename;

        public readonly char Separator;

        private readonly StreamReader Reader;

        private FileReader(string filename, char separator)
        {
            Filename = filename;
            Separator = separator;

            Reader = new(filename);
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

        // remove from list
        public void Dispose()
        {
            Reader.Close();
        }

        public string? ReadLn() => Reader.ReadLine();
        
        public PointType3D? GetXYZ()
        {
            var str = Reader.ReadLine();
            PointType3D? res = default;

            if (str is not null)
            {
                var data = str.Split(Separator);

                if (data.Length == 3)
                {
                    res = new PointType3D
                    {
                        X = (CoordType)Convert.ChangeType(data[0], typeof(CoordType)),
                        Y = (CoordType)Convert.ChangeType(data[1], typeof(CoordType)),
                        Z = (CoordType)Convert.ChangeType(data[2], typeof(CoordType))
                    };
                }
            }

            return res;
        }

        public PointType2D? GetXY()
        {
            var point3D = GetXYZ();

            PointType2D? res = default;

            if (point3D is not null)
            {
                res = new PointType2D
                {
                    X = point3D.X,
                    Y = point3D.Y
                };
            }

            return res;
        }

        public PointType2D? GetYZ()
        {
            var point3D = GetXYZ();

            PointType2D? res = default;

            if (point3D is not null)
            {
                res = new PointType2D
                {
                    X = point3D.Y,
                    Y = point3D.Z
                };
            }

            return res;
        }

        public PointType2D? GetXZ()
        {
            var point3D = GetXYZ();

            PointType2D? res = default;

            if (point3D is not null)
            {
                res = new PointType2D
                {
                    X = point3D.X,
                    Y = point3D.Z
                };
            }

            return res;
        }

        public PointType2D? GetYX()
        {
            var point3D = GetXYZ();

            PointType2D? res = default;

            if (point3D is not null)
            {
                res = new PointType2D
                {
                    X = point3D.Y,
                    Y = point3D.X
                };
            }

            return res;
        }

        public PointType2D? GetZY()
        {
            var point3D = GetXYZ();

            PointType2D? res = default;

            if (point3D is not null)
            {
                res = new PointType2D
                {
                    X = point3D.Z,
                    Y = point3D.Y
                };
            }

            return res;
        }

        public PointType2D? GetZX()
        {
            var point3D = GetXYZ();

            PointType2D? res = default;

            if (point3D is not null)
            {
                res = new PointType2D
                {
                    X = point3D.Z,
                    Y = point3D.X
                };
            }

            return res;
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
