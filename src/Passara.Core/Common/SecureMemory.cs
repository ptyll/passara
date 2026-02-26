using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Passara.Core.Common;

/// <summary>
/// Provides a secure way to store sensitive data in memory with automatic zeroization.
/// </summary>
/// <remarks>
/// This is a modern replacement for the deprecated SecureString.
/// Data is pinned in memory and zeroed when disposed.
/// </remarks>
public sealed class SecureMemory : IDisposable
{
    private byte[]? _buffer;
    private GCHandle _handle;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecureMemory"/> class with the specified data.
    /// </summary>
    /// <param name="data">The sensitive data to store. A copy is made and the original should be cleared.</param>
    /// <exception cref="ArgumentNullException">Thrown when data is null.</exception>
    public SecureMemory(byte[] data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        _buffer = new byte[data.Length];
        Buffer.BlockCopy(data, 0, _buffer, 0, data.Length);
        _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecureMemory"/> class with the specified string.
    /// </summary>
    /// <param name="data">The sensitive string to store.</param>
    /// <exception cref="ArgumentNullException">Thrown when data is null.</exception>
    public SecureMemory(string data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        _buffer = System.Text.Encoding.UTF8.GetBytes(data);
        _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
    }

    /// <summary>
    /// Gets the length of the stored data in bytes.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed.</exception>
    public int Length
    {
        get
        {
            ThrowIfDisposed();
            return _buffer?.Length ?? 0;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the secure memory has been disposed.
    /// </summary>
    public bool IsDisposed => _disposed;

    /// <summary>
    /// Executes the specified action with a copy of the secured data.
    /// The copy is automatically cleared after the action completes.
    /// </summary>
    /// <typeparam name="T">The return type of the action.</typeparam>
    /// <param name="action">The action to execute with the data.</param>
    /// <returns>The result of the action.</returns>
    /// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed.</exception>
    public T Use<T>(Func<byte[], T> action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        ThrowIfDisposed();

        byte[]? copy = null;
        try
        {
            copy = new byte[_buffer!.Length];
            Buffer.BlockCopy(_buffer, 0, copy, 0, _buffer.Length);
            return action(copy);
        }
        finally
        {
            if (copy is not null)
            {
                ZeroMemory(copy);
            }
        }
    }

    /// <summary>
    /// Executes the specified action with a copy of the secured data.
    /// The copy is automatically cleared after the action completes.
    /// </summary>
    /// <param name="action">The action to execute with the data.</param>
    /// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed.</exception>
    public void Use(Action<byte[]> action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        ThrowIfDisposed();

        byte[]? copy = null;
        try
        {
            copy = new byte[_buffer!.Length];
            Buffer.BlockCopy(_buffer, 0, copy, 0, _buffer.Length);
            action(copy);
        }
        finally
        {
            if (copy is not null)
            {
                ZeroMemory(copy);
            }
        }
    }

    /// <summary>
    /// Copies the secured data to a new array.
    /// The caller is responsible for clearing the returned array.
    /// </summary>
    /// <returns>A copy of the secured data.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed.</exception>
    [Obsolete("Use Use() or Use<T>() instead to ensure proper cleanup.")]
    public byte[] ToByteArray()
    {
        ThrowIfDisposed();

        var copy = new byte[_buffer!.Length];
        Buffer.BlockCopy(_buffer, 0, copy, 0, _buffer.Length);
        return copy;
    }

    /// <summary>
    /// Converts the secured data to a string.
    /// </summary>
    /// <returns>The string representation of the data.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed.</exception>
    public override string ToString()
    {
        return Use(bytes => System.Text.Encoding.UTF8.GetString(bytes));
    }

    /// <summary>
    /// Releases all resources used by the <see cref="SecureMemory"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="SecureMemory"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Zero out the buffer before freeing
                if (_buffer is not null)
                {
                    ZeroMemory(_buffer);
                }
            }

            // Free the pinned handle
            if (_handle.IsAllocated)
            {
                _handle.Free();
            }

            _buffer = null;
            _disposed = true;
        }
    }

    /// <summary>
    /// Finalizer to ensure cleanup if Dispose is not called.
    /// </summary>
    ~SecureMemory()
    {
        Dispose(false);
    }

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if the object has been disposed.
    /// </summary>
    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(SecureMemory));
        }
    }

    /// <summary>
    /// Securely zeros the contents of a byte array.
    /// </summary>
    private static void ZeroMemory(byte[] buffer)
    {
        if (buffer is null || buffer.Length == 0)
        {
            return;
        }

        // Use RandomNumberGenerator for secure zeroing (constant-time where possible)
        // or manually clear in a way that's less likely to be optimized away
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = 0;
        }
    }
}

/// <summary>
/// Provides extension methods for working with secure memory.
/// </summary>
public static class SecureMemoryExtensions
{
    /// <summary>
    /// Converts a string to a <see cref="SecureMemory"/> instance.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>A new <see cref="SecureMemory"/> containing the string data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    public static SecureMemory ToSecureMemory(this string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return new SecureMemory(value);
    }

    /// <summary>
    /// Converts a byte array to a <see cref="SecureMemory"/> instance.
    /// </summary>
    /// <param name="value">The byte array to convert.</param>
    /// <returns>A new <see cref="SecureMemory"/> containing the byte data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    public static SecureMemory ToSecureMemory(this byte[] value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return new SecureMemory(value);
    }
}
