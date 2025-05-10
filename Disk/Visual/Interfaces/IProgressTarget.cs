namespace Disk.Visual.Interfaces;

public interface IProgressTarget : ITarget
{
    /// <summary>
    ///     Current progress
    /// </summary>
    double Progress { get; }

    /// <summary>
    ///     Checks if <see cref="Progress"/> is full
    /// </summary>
    bool IsFull { get; }

    /// <summary>
    ///     Set <see cref="Progress"/> value to 0
    /// </summary>
    void Reset();
}
