namespace Passara.Core.Cryptography;

/// <summary>
/// A generic wrapper for sensitive data that automatically cleans up when disposed.
/// </summary>
/// <typeparam name="T">The type of the sensitive data.</typeparam>
public sealed class SensitiveData<T> : IDisposable where T : class
{
    private T? _data;
    private readonly Action<T>? _cleanup;
    private bool _disposed;

    private SensitiveData(T data, Action<T>? cleanup = null)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        _cleanup = cleanup;
    }

    /// <summary>
    /// Creates a new instance of <see cref="SensitiveData{T}"/>.
    /// </summary>
    /// <param name="data">The sensitive data to wrap.</param>
    /// <param name="valueSelector">A function to extract a value from the data.</param>
    /// <param name="cleanup">Optional cleanup action to call on disposal.</param>
    /// <returns>A new <see cref="SensitiveData{T}"/> instance.</returns>
    public static SensitiveData<T> Create(T data, Func<T, object>? valueSelector = null, Action<T>? cleanup = null)
    {
        return new SensitiveData<T>(data, cleanup);
    }

    /// <summary>
    /// Gets a value extracted from the sensitive data.
    /// </summary>
    public object? Value
    {
        get
        {
            ThrowIfDisposed();
            return _data;
        }
    }

    /// <summary>
    /// Uses the sensitive data with the specified action.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="action">The action to execute with the data.</param>
    /// <returns>The result of the action.</returns>
    public TResult Use<TResult>(Func<T, TResult> action)
    {
        ThrowIfDisposed();
        return action(_data!);
    }

    /// <summary>
    /// Uses the sensitive data with the specified action.
    /// </summary>
    /// <param name="action">The action to execute with the data.</param>
    public void Use(Action<T> action)
    {
        ThrowIfDisposed();
        action(_data!);
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
                _cleanup?.Invoke(_data!);
            }

            _data = null;
            _disposed = true;
        }
    }

    /// <summary>
    /// Finalizer to ensure cleanup if Dispose is not called.
    /// </summary>
    ~SensitiveData()
    {
        Dispose(false);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(SensitiveData<T>));
        }
    }
}
