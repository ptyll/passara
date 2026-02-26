using System.Runtime.InteropServices;

namespace Passara.Core.Cryptography;

/// <summary>
/// Represents a master encryption key with secure memory management.
/// </summary>
public sealed class MasterKey : IDisposable
{
    private byte[]? _key;
    private GCHandle _handle;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MasterKey"/> class with the specified key.
    /// </summary>
    /// <param name="key">The master key. A copy is made.</param>
    /// <exception cref="ArgumentNullException">Thrown when key is null.</exception>
    /// <exception cref="ArgumentException">Thrown when key length is invalid.</exception>
    public MasterKey(byte[] key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (key.Length != EncryptionConstants.MasterKeyLength)
        {
            throw new ArgumentException($"Key must be exactly {EncryptionConstants.MasterKeyLength} bytes.", nameof(key));
        }

        _key = new byte[key.Length];
        Buffer.BlockCopy(key, 0, _key, 0, key.Length);
        _handle = GCHandle.Alloc(_key, GCHandleType.Pinned);
    }

    /// <summary>
    /// Gets the length of the key in bytes.
    /// </summary>
#pragma warning disable CA1822
    public int Length => EncryptionConstants.MasterKeyLength;
#pragma warning restore CA1822

    /// <summary>
    /// Gets a value indicating whether the key has been disposed.
    /// </summary>
    public bool IsDisposed => _disposed;

    /// <summary>
    /// Uses the key with the specified action.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="action">The action to execute with the key.</param>
    /// <returns>The result of the action.</returns>
    public T Use<T>(Func<byte[], T> action)
    {
        ThrowIfDisposed();

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        return action(_key!);
    }

    /// <summary>
    /// Uses the key with the specified action.
    /// </summary>
    /// <param name="action">The action to execute with the key.</param>
    public void Use(Action<byte[]> action)
    {
        ThrowIfDisposed();

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        action(_key!);
    }

    /// <summary>
    /// Creates a new random master key.
    /// </summary>
    /// <returns>A new random master key.</returns>
    public static MasterKey CreateRandom()
    {
        var key = new byte[EncryptionConstants.MasterKeyLength];
        System.Security.Cryptography.RandomNumberGenerator.Fill(key);
        return new MasterKey(key);
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
                // Zero out the key
                if (_key != null)
                {
                    ZeroMemory(_key);
                }
            }

            // Free the pinned handle
            if (_handle.IsAllocated)
            {
                _handle.Free();
            }

            _key = null;
            _disposed = true;
        }
    }

    /// <summary>
    /// Finalizer to ensure cleanup if Dispose is not called.
    /// </summary>
    ~MasterKey()
    {
        Dispose(false);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(MasterKey));
        }
    }

    private static void ZeroMemory(byte[] buffer)
    {
        if (buffer == null || buffer.Length == 0)
        {
            return;
        }

        // Use CryptographicOperations.ZeroMemory if available
#if NET6_0_OR_GREATER
        System.Security.Cryptography.CryptographicOperations.ZeroMemory(buffer);
#else
        // Manual zeroing for older frameworks
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = 0;
        }
#endif
    }
}
