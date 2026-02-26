using FluentAssertions;
using Passara.Core.Common;
using Xunit;

namespace Passara.Desktop.Tests.Common;

public class ResultTests : UnitTestBase
{
    #region Result (non-generic)

    [Fact]
    public void Success_ReturnsSuccessResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCode.None);
    }

    [Fact]
    public void Failure_ReturnsFailureResult()
    {
        // Act
        var result = Result.Failure(ErrorCode.InvalidMasterPassword);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be(ErrorCode.InvalidMasterPassword);
    }

    [Fact]
    public void Failure_WithMessage_ReturnsFailureResultWithMessage()
    {
        // Arrange
        const string message = "Custom error message";

        // Act
        var result = Result.Failure(ErrorCode.VaultNotFound, message);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCode.VaultNotFound);
        result.ErrorMessage.Should().Be(message);
    }

    [Fact]
    public void Failure_WithEmptyMessage_ThrowsArgumentException()
    {
        // Act
        Action act = () => Result.Failure(ErrorCode.UnknownError, string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equals_SameSuccessResults_AreEqual()
    {
        // Arrange
        var result1 = Result.Success();
        var result2 = Result.Success();

        // Assert
        result1.Should().Be(result2);
        (result1 == result2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentFailureResults_AreNotEqual()
    {
        // Arrange
        var result1 = Result.Failure(ErrorCode.InvalidMasterPassword);
        var result2 = Result.Failure(ErrorCode.VaultNotFound);

        // Assert
        result1.Should().NotBe(result2);
        (result1 != result2).Should().BeTrue();
    }

    #endregion

    #region Result<T> (generic)

    [Fact]
    public void Success_WithValue_ReturnsSuccessResultWithValue()
    {
        // Arrange
        const string value = "test value";

        // Act
        var result = Result<string>.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void Success_WithNullValue_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => Result<string>.Success(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Failure_Generic_ReturnsFailureResult()
    {
        // Act
        var result = Result<string>.Failure(ErrorCode.EncryptionFailed);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCode.EncryptionFailed);
        result.Value.Should().BeNull();
    }

    [Fact]
    public void ImplicitConversion_FromValue_CreatesSuccessResult()
    {
        // Arrange
        const int value = 42;

        // Act
        Result<int> result = value;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void ImplicitConversion_ToValue_ReturnsValue()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        int value = result;

        // Assert
        value.Should().Be(42);
    }

    [Fact]
    public void ImplicitConversion_ToValue_WhenFailure_ThrowsInvalidOperationException()
    {
        // Arrange
        var result = Result<int>.Failure(ErrorCode.VaultLocked);

        // Act
        Action act = () => { int _ = result; };

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ToString_Success_ReturnsSuccessString()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        var str = result.ToString();

        // Assert
        str.Should().Be("Success(42)");
    }

    [Fact]
    public void ToString_Failure_ReturnsFailureString()
    {
        // Arrange
        var result = Result<int>.Failure(ErrorCode.DiskFull);

        // Act
        var str = result.ToString();

        // Assert
        str.Should().Be("Failure(DiskFull)");
    }

    #endregion
}
