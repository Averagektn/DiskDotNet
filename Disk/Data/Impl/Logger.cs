using Disk.Data.Interface;
using System.IO;

namespace Disk.Data.Impl;

/// <summary>
///     Represents a logger class that logs data to a file.
/// </summary>
public class Logger : ILogger, IDisposable
{
    /// <summary>
    ///     Gets the filename associated with the logger.
    /// </summary>
    public readonly string Filename;

    private static readonly List<Logger> Loggers = [];

    private readonly StreamWriter Writer;

    private static object _lock = new object();

    /// <summary>
    ///     Initializes a new instance of the <see cref="Logger"/> class with the specified filename.
    /// </summary>
    /// <param name="filename">
    ///     The filename to associate with the logger
    /// </param>
    private Logger(string filename)
    {
        Filename = filename;
        Writer = new StreamWriter(filename);
    }

    /// <summary>
    ///     Gets a logger instance associated with the specified filename. 
    ///     If a logger with the same filename already exists, that instance is returned; 
    ///     otherwise, a new instance is created
    /// </summary>
    /// <param name="filename">
    ///     The filename associated with the logger
    /// </param>
    /// <returns>
    ///     A logger instance associated with the specified filename
    /// </returns>
    public static Logger GetLogger(string filename)
    {
        var logger = Loggers.FirstOrDefault(s => s.Filename == filename);

        if (logger is null)
        {
            lock (_lock)
            {
                logger = new Logger(filename);
                Loggers.Add(logger);
            }
        }

        return logger;
    }

    /// <summary>
    ///     Disposes the logger and removes it from the list of loggers
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _ = Loggers.Remove(this);
        Writer.Close();
    }

    /// <inheritdoc/>
    public void Log(object? data) => Writer.Write(data?.ToString());

    /// <inheritdoc/>
    public async Task LogAsync(object? data) => await Writer.WriteAsync(data?.ToString());

    /// <inheritdoc/>
    public void LogLn(object? data) => Writer.WriteLine(data?.ToString());

    /// <inheritdoc/>
    public async Task LogLnAsync(object? data) => await Writer.WriteLineAsync(data?.ToString());
}
