namespace Passara.Core.Cryptography;

/// <summary>
/// Provides a secure buffer for storing sensitive data in memory.
/// </summary>
/// <remarks>
/// Implementations must ensure that:
/// 1. Memory is pinned to prevent swapping to disk
/// 2. Memory is zeroized upon disposal
/// 3. No copies of the data are left in managed memory
/// </remarks>
public interface ISecureBuffer : IDisposable
{
    /// <summary>
    /// Gets the size of the buffer in bytes.
    /// </summary>
    int Size { get; }

    /// <summary>
    /// Gets a value indicating whether the buffer has been disposed.
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// Executes the specified action with a read-only copy of the buffer contents.
    /// The copy is automatically cleared after the action completes.
    /// </summary>
    /// <typeparam name="T">The return type of the action.</typeparam>
    /// <param name="action">The action to execute with the data.</param>
    /// <returns>The result of the action.</returns>
    T UseRead<T>(Func<byte[], T> action);

    /// <summary>
    /// Executes the specified action with a read-only copy of the buffer contents.
    /// The copy is automatically cleared after the action completes.
    /// </summary>
    /// <param name="action">The action to execute with the data.</param>
    void UseRead(Action<byte[]> action);

    /// <summary>
    /// Executes the specified action with a writable copy of the buffer contents.
    /// The modified copy is written back to the buffer and then cleared.
    /// </summary>
    /// <typeparam name="T">The return type of the action.</typeparam>
    /// <param name="action">The action to execute with the data.</param>
    /// <returns>The result of the action.</returns>
    T UseWrite<T>(Func<byte[], T> action);

    /// <summary>
    /// Executes the specified action with a writable copy of the buffer contents.
    /// The modified copy is written back to the buffer and then cleared.
    /// </summary>
    /// <param name="action">The action to execute with the data.</param>
    void UseWrite(Action<byte[]> action);
}
