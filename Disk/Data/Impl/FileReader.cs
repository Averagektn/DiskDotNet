using Disk.Data.Interface;
using System.IO;

namespace Disk.Data.Impl
{
    /// <summary>
    ///     Represents a file reader for a specific coordinate type
    /// </summary>
    /// <typeparam name="CoordType">
    ///     The coordinate type
    /// </typeparam>
    class FileReader<CoordType> :
        IDataSource<CoordType>,
        IDisposable
        where CoordType :
            IConvertible,
            new()
    {
        /// <summary>
        ///     Gets the filename associated with the file reader.
        /// </summary>
        public readonly string Filename;

        /// <summary>
        ///     Gets the separator used in the file.
        /// </summary>
        public readonly char Separator;

        /// <summary>
        ///     Holds a list of active file readers.
        /// </summary>
        private static readonly List<FileReader<CoordType>> Files = [];

        /// <summary>
        ///     The underlying StreamReader used for reading the file.
        /// </summary>
        private readonly StreamReader Reader;

        /// <summary>
        ///     Initializes a new instance of the FileReader class with the specified filename and separator.
        /// </summary>
        /// <param name="filename">
        ///     The name of the file to be read
        /// </param>
        /// <param name="separator">
        ///     The character used as a separator in the file
        /// </param>
        private FileReader(string filename, char separator)
        {
            Filename = filename;
            Separator = separator;

            if (!File.Exists(filename))
            {
                File.Create(filename);
            }

            Reader = new StreamReader(filename);
        }

        /// <summary>
        ///     Opens a file reader for the specified filename and separator<br/>
        ///     If a file reader for the same filename already exists, it returns the existing instance<br/>
        /// </summary>
        /// <param name="filename">
        ///     The name of the file to be read
        /// </param>
        /// <param name="separator">
        ///     The character used as a separator in the file
        /// </param>
        /// <returns>
        ///     The file reader instance
        /// </returns>
        public static FileReader<CoordType> Open(string filename, char separator = ';')
        {
            var reader = Files.FirstOrDefault(f => f.Filename == filename);

            if (reader is null)
            {
                reader = new FileReader<CoordType>(filename, separator);
                Files.Add(reader);
            }

            return reader;
        }

        /// <summary>
        ///     Disposes of the file reader, closing the underlying StreamReader and removing it from 
        ///     the active file readers list
        /// </summary>
        public void Dispose()
        {
            Files.Remove(this);
            Reader.Close();
        }

        /// <summary>
        ///     Reads a line from the file
        /// </summary>
        /// <returns>
        ///     The read line as a string
        /// </returns>
        public string? ReadLn() => Reader.ReadLine();

        /// <summary>
        ///     Gets the next XYZ point from the file
        /// </summary>
        /// <returns>
        ///     The XYZ point as a Point3D instance
        /// </returns>
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
                        (CoordType)Convert.ChangeType(data[2], typeof(CoordType)));
                }
            }

            return res;
        }

        /// <summary>
        ///     Gets the next XY point from the file
        /// </summary>
        /// <returns>
        ///     The XY point as a Point2D instance
        /// </returns>
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
                        (CoordType)Convert.ChangeType(data[1], typeof(CoordType)));
                }
            }

            return res;
        }

        /// <summary>
        ///     Gets the next YZ point from the file  
        /// </summary>
        /// <returns>
        ///     The YZ point as a Point2D instance
        /// </returns>
        public Point2D<CoordType>? GetYZ()
        {
            var point3D = GetXYZ();

            Point2D<CoordType>? res = null;

            if (point3D is not null)
            {
                res = new Point2D<CoordType>(point3D.Y, point3D.Z);
            }

            return res;
        }

        /// <summary>
        ///     Retrieves a 2D point in the XZ plane
        /// </summary>
        /// <returns>
        ///     The 2D point in the XZ plane, or null if the 3D point is null
        /// </returns>
        public Point2D<CoordType>? GetXZ()
        {
            var point3D = GetXYZ();

            Point2D<CoordType>? res = null;

            if (point3D is not null)
            {
                res = new Point2D<CoordType>(point3D.X, point3D.Z);
            }

            return res;
        }

        /// <summary>
        ///     Retrieves a 2D point in the YX plane
        /// </summary>
        /// <returns>
        ///     The 2D point in the YX plane, or null if the 2D point is null
        /// </returns>
        public Point2D<CoordType>? GetYX()
        {
            var point2D = GetXY();

            return point2D is null ? point2D : new Point2D<CoordType>(point2D.Y, point2D.X);
        }

        /// <summary>
        ///     Retrieves a 2D point in the ZY plane
        /// </summary>
        /// <returns>
        ///     The 2D point in the ZY plane, or null if the 3D point is null
        /// </returns>
        public Point2D<CoordType>? GetZY()
        {
            var point3D = GetXYZ();

            Point2D<CoordType>? res = null;

            if (point3D is not null)
            {
                res = new Point2D<CoordType>(point3D.Z, point3D.Y);
            }

            return res;
        }

        /// <summary>
        ///     Retrieves a 2D point in the ZX plane
        /// </summary>
        /// <returns>
        ///     The 2D point in the ZX plane, or null if the 3D point is null
        /// </returns>
        public Point2D<CoordType>? GetZX()
        {
            var point3D = GetXYZ();

            Point2D<CoordType>? res = null;

            if (point3D is not null)
            {
                res = new Point2D<CoordType>(point3D.Z, point3D.X);
            }

            return res;
        }

        /// <summary>
        ///     Retrieves a collection of 2D points based on the specified parameters
        /// </summary>
        /// <param name="isX">
        ///     Specifies whether to include the X coordinate in the 2D points
        /// </param>
        /// <param name="isY">
        ///     Specifies whether to include the Y coordinate in the 2D points
        /// </param>
        /// <param name="isZ">
        ///     Specifies whether to include the Z coordinate in the 2D points
        /// </param>
        /// <param name="isStraightforward">
        ///     Specifies whether to use the straightforward order of coordinates (XY, YZ, XZ) or the reversed order 
        ///     (YX, ZY, ZX)
        /// </param>
        /// <returns>
        ///     A collection of 2D points based on the specified parameters
        /// </returns>
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

        /// <summary>
        ///     Retrieves a collection of 3D points 
        /// </summary>
        /// <returns>
        ///     A collection of 3D points
        /// </returns>
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
