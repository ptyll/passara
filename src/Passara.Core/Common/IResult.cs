namespace Passara.Core.Common;

/// <summary>
/// Represents the result of an operation that may succeed or fail.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error code if the operation failed.
    /// </summary>
    ErrorCode ErrorCode { get; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    string? ErrorMessage { get; }
}

/// <summary>
/// Represents the result of an operation that returns a value and may succeed or fail.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public interface IResult<out T> : IResult
{
    /// <summary>
    /// Gets the value if the operation was successful.
    /// </summary>
    T? Value { get; }
}
