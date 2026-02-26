namespace Passara.Core.Common;

/// <summary>
/// Represents the result of an operation that may succeed or fail.
/// </summary>
public readonly struct Result : IResult, IEquatable<Result>
{
    /// <inheritdoc />
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <inheritdoc />
    public ErrorCode ErrorCode { get; }

    /// <inheritdoc />
    public string? ErrorMessage { get; }

    private Result(bool isSuccess, ErrorCode errorCode, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true, ErrorCode.None, null);

    /// <summary>
    /// Creates a failed result with the specified error code.
    /// </summary>
    public static Result Failure(ErrorCode errorCode) => new(false, errorCode, null);

    /// <summary>
    /// Creates a failed result with the specified error code and message.
    /// </summary>
    public static Result Failure(ErrorCode errorCode, string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException("Error message cannot be null or empty.", nameof(errorMessage));
        }

        return new Result(false, errorCode, errorMessage);
    }

    /// <inheritdoc />
    public bool Equals(Result other) => IsSuccess == other.IsSuccess && ErrorCode == other.ErrorCode;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Result other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(IsSuccess, ErrorCode);

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(Result left, Result right) => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(Result left, Result right) => !left.Equals(right);
}

/// <summary>
/// Represents the result of an operation that returns a value and may succeed or fail.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public readonly struct Result<T> : IResult<T>, IEquatable<Result<T>>
{
    /// <inheritdoc />
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <inheritdoc />
    public ErrorCode ErrorCode { get; }

    /// <inheritdoc />
    public string? ErrorMessage { get; }

    /// <inheritdoc />
    public T? Value { get; }

    private Result(bool isSuccess, ErrorCode errorCode, string? errorMessage, T? value)
    {
        IsSuccess = isSuccess;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Value = value;
    }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    public static Result<T> Success(T value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value), "Success value cannot be null.");
        }

        return new Result<T>(true, ErrorCode.None, null, value);
    }

    /// <summary>
    /// Creates a failed result with the specified error code.
    /// </summary>
    public static Result<T> Failure(ErrorCode errorCode) => new(false, errorCode, null, default);

    /// <summary>
    /// Creates a failed result with the specified error code and message.
    /// </summary>
    public static Result<T> Failure(ErrorCode errorCode, string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException("Error message cannot be null or empty.", nameof(errorMessage));
        }

        return new Result<T>(false, errorCode, errorMessage, default);
    }

    /// <summary>
    /// Implicitly converts a value to a successful result.
    /// </summary>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Implicitly converts a result to its value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the result is not successful.</exception>
    public static implicit operator T?(Result<T> result) => result.IsSuccess
        ? result.Value
        : throw new InvalidOperationException($"Cannot extract value from failed result. Error: {result.ErrorCode}");

    /// <inheritdoc />
    public bool Equals(Result<T> other)
    {
        if (IsSuccess != other.IsSuccess || ErrorCode != other.ErrorCode)
        {
            return false;
        }

        if (Value is null)
        {
            return other.Value is null;
        }

        return other.Value is not null && EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Result<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(IsSuccess, ErrorCode, Value);

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);

    /// <inheritdoc />
    public override string ToString() => IsSuccess
        ? $"Success({Value})"
        : $"Failure({ErrorCode})";
}
