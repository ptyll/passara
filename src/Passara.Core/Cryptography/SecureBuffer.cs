using System.Runtime.InteropServices;
using Passara.Core.Common;

namespace Passara.Core.Cryptography;

/// <summary>
/// Provides a secure buffer implementation using pinned managed memory.
/// </summary>
/// <remarks>
/// This implementation uses:
/// 1. Pinned memory to prevent GC movement
/// 2. Manual zeroization on dispose
/// 3. No copies of the data are left in managed memory
/// </remarks>
public sealed class SecureBuffer : ISecureBuffer
{
    private byte[]? _buffer;
    private GCHandle _handle;
    private readonly int _size;
    private bool _disposed;

    // Maximum allocation size: 1GB
    private const int MaxSize = 1024 * 1024 * 1024;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecureBuffer"/> class with the specified size.
    /// </summary>
    /// <param name="size">The size of the buffer in bytes.</param>
    /// <exception cref="ArgumentException">Thrown when size is negative or exceeds maximum.</exception>
    public SecureBuffer(int size)
    {
        if (size < 0)
        {
            throw new ArgumentException("Size cannot be negative.", nameof(size));
        }

        if (size > MaxSize)
        {
            throw new ArgumentException($"Size cannot exceed {MaxSize} bytes.", nameof(size));
        }

        _size = size;

        if (size > 0)
        {
            try
            {
                _buffer = new byte[size];
                _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
            }
            catch (OutOfMemoryException)
            {
                throw new InvalidOperationException("Failed to allocate secure memory.");
            }
        }
    }

    /// <inheritdoc />
    public int Size => _size;

    /// <inheritdoc />
    public bool IsDisposed => _disposed;

    /// <inheritdoc />
    public T UseRead<T>(Func<byte[], T> action)
    {
        ThrowIfDisposed();

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (_size == 0)
        {
            return action(Array.Empty<byte>());
        }

        byte[]? copy = null;
        try
        {
            copy = new byte[_size];
            Buffer.BlockCopy(_buffer!, 0, copy, 0, _size);
            return action(copy);
        }
        finally
        {
            if (copy != null)
            {
                ZeroMemory(copy);
            }
        }
    }

    /// <inheritdoc />
    public void UseRead(Action<byte[]> action)
    {
        ThrowIfDisposed();

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (_size == 0)
        {
            action(Array.Empty<byte>());
            return;
        }

        byte[]? copy = null;
        try
        {
            copy = new byte[_size];
            Buffer.BlockCopy(_buffer!, 0, copy, 0, _size);
            action(copy);
        }
        finally
        {
            if (copy != null)
            {
                ZeroMemory(copy);
            }
        }
    }

    /// <inheritdoc />
    public T UseWrite<T>(Func<byte[], T> action)
    {
        ThrowIfDisposed();

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (_size == 0)
        {
            return action(Array.Empty<byte>());
        }

        byte[]? copy = null;
        try
        {
            copy = new byte[_size];
            Buffer.BlockCopy(_buffer!, 0, copy, 0, _size);
            var result = action(copy);
            Buffer.BlockCopy(copy, 0, _buffer!, 0, _size);
            return result;
        }
        finally
        {
            if (copy != null)
            {
                ZeroMemory(copy);
            }
        }
    }

    /// <inheritdoc />
    public void UseWrite(Action<byte[]> action)
    {
        ThrowIfDisposed();

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (_size == 0)
        {
            action(Array.Empty<byte>());
            return;
        }

        byte[]? copy = null;
        try
        {
            copy = new byte[_size];
            Buffer.BlockCopy(_buffer!, 0, copy, 0, _size);
            action(copy);
            Buffer.BlockCopy(copy, 0, _buffer!, 0, _size);
        }
        finally
        {
            if (copy != null)
            {
                ZeroMemory(copy);
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Zero out the buffer
                if (_buffer != null && _size > 0)
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
    ~SecureBuffer()
    {
        Dispose(false);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(SecureBuffer));
        }
    }

    /// <summary>
    /// Securely zeros the contents of a byte array.
    /// </summary>
    private static void ZeroMemory(byte[] buffer)
    {
        if (buffer == null || buffer.Length == 0)
        {
            return;
        }

        // Manual zeroing
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = 0;
        }
    }
}
