using Disk.Data.Interface;
using System.IO;

namespace Disk.Data.Impl
{
    class FileReader<CoordType> :
        IDataSource<CoordType>,
        IDisposable
        where CoordType :
            IConvertible,
            new()
    {
        public readonly string Filename;
        public readonly char Separator;

        private static readonly List<FileReader<CoordType>> Files = [];

        private readonly StreamReader Reader;

        private FileReader(string filename, char separator)
        {
            Filename = filename;
            Separator = separator;

            if (!File.Exists(filename))
            {
                File.Create(filename);
            }

            Reader = new(filename);
        }

        public static FileReader<CoordType> Open(string filename, char separator)
        {
            var reader = Files.FirstOrDefault(f => f.Filename == filename);

            if (reader is null)
            {
                reader = new(filename, separator);
                Files.Add(reader);
            }

            return reader;
        }

        public void Dispose()
        {
            Files.Remove(this);

            Reader.Close();
        }

        public string? ReadLn() => Reader.ReadLine();

        public Point3D<CoordType>? GetXYZ()
        {
            var str = Reader.ReadLine();
            Point3D<CoordType>? res = null;

            if (str is not null)
            {
                var data = str.Split(Separator);

                if (data.Length == 3)
                {
                    res = new Point3D<CoordType>(
                        (CoordType)Convert.ChangeType(data[0], typeof(CoordType)),
                        (CoordType)Convert.ChangeType(data[1], typeof(CoordType)),
                        (CoordType)Convert.ChangeType(data[2], typeof(CoordType))
                        );
                }
            }

            return res;
        }

        public Point2D<CoordType>? GetXY()
        {
            var str = Reader.ReadLine();
            Point2D<CoordType>? res = null;

            if (str is not null)
            {
                var data = str.Split(Separator);

                if (data.Length == 2)
                {
                    res = new Point2D<CoordType>(
                        (CoordType)Convert.ChangeType(data[0], typeof(CoordType)),
                        (CoordType)Convert.ChangeType(data[1], typeof(CoordType))
                        );
                }
            }

            return res;
        }

        public Point2D<CoordType>? GetYZ()
        {
            var point3D = GetXYZ();

            Point2D<CoordType>? res = null;

            if (point3D is not null)
            {
                res = new(point3D.Y, point3D.Z);
            }

            return res;
        }

        public Point2D<CoordType>? GetXZ()
        {
            var point3D = GetXYZ();

            Point2D<CoordType>? res = null;

            if (point3D is not null)
            {
                res = new(point3D.X, point3D.Z);
            }

            return res;
        }

        public Point2D<CoordType>? GetYX()
        {
            var point2D = GetXY();

            return point2D is null ? point2D : new(point2D.Y, point2D.X);
        }

        public Point2D<CoordType>? GetZY()
        {
            var point3D = GetXYZ();

            Point2D<CoordType>? res = null;

            if (point3D is not null)
            {
                res = new(point3D.Z, point3D.Y);
            }

            return res;
        }

        public Point2D<CoordType>? GetZX()
        {
            var point3D = GetXYZ();

            Point2D<CoordType>? res = null;

            if (point3D is not null)
            {
                res = new(point3D.Z, point3D.X);
            }

            return res;
        }

        public IEnumerable<Point2D<CoordType>> Get2DPoints(bool isX = true, bool isY = true, bool isZ = false,
            bool isStraightforward = true)
        {
            Point2D<CoordType>? p = null;

            do
            {
                if (isStraightforward)
                {
                    if (isX && isY)
                    {
                        p = GetXY();
                    }
                    else if (isY && isZ)
                    {
                        p = GetYZ();
                    }
                    else if (isX && isZ)
                    {
                        p = GetXZ();
                    }
                }
                else
                {
                    if (isX && isY)
                    {
                        p = GetYX();
                    }
                    else if (isY && isZ)
                    {
                        p = GetZY();
                    }
                    else if (isX && isZ)
                    {
                        p = GetZX();
                    }
                }

                if (p is not null)
                {
                    yield return p;
                }
                else
                {
                    yield break;
                }
            } while (true);
        }

        public IEnumerable<Point3D<CoordType>> Get3DPoints()
        {
            do
            {
                var p = GetXYZ();

                if (p is not null)
                {
                    yield return p;
                }
                else
                {
                    yield break;
                }
            } while (true);
        }
    }
}
