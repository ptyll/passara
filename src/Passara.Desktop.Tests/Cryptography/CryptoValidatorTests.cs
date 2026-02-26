using FluentAssertions;
using Passara.Core.Cryptography;
using Xunit;

namespace Passara.Desktop.Tests.Cryptography;

public class CryptoValidatorTests : UnitTestBase
{
    private readonly CryptoValidator _validator;

    public CryptoValidatorTests()
    {
        _validator = new CryptoValidator();
    }

    #region Key Length Tests

    [Theory]
    [InlineData(16)]
    [InlineData(24)]
    [InlineData(32)]
    public void ValidateKeyLength_ValidLengths_ReturnsTrue(int length)
    {
        // Act
        var result = _validator.ValidateKeyLength(length);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    [InlineData(17)]
    [InlineData(33)]
    [InlineData(64)]
    public void ValidateKeyLength_InvalidLengths_ReturnsFalse(int length)
    {
        // Act
        var result = _validator.ValidateKeyLength(length);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    #endregion

    #region Salt Length Tests

    [Fact]
    public void ValidateSaltLength_CorrectLength_ReturnsTrue()
    {
        // Act
        var result = _validator.ValidateSaltLength(KdfParameters.SaltLength);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(8)]
    [InlineData(15)]
    [InlineData(17)]
    [InlineData(64)]
    public void ValidateSaltLength_InvalidLengths_ReturnsFalse(int length)
    {
        // Act
        var result = _validator.ValidateSaltLength(length);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    #endregion

    #region Nonce Length Tests

    [Theory]
    [InlineData(12, CipherAlgorithm.Aes256Gcm)]
    [InlineData(8, CipherAlgorithm.ChaCha20Poly1305)]
    public void ValidateNonceLength_ValidLength_ReturnsTrue(int length, CipherAlgorithm algorithm)
    {
        // Act
        var result = _validator.ValidateNonceLength(length, algorithm);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(0, CipherAlgorithm.Aes256Gcm)]
    [InlineData(11, CipherAlgorithm.Aes256Gcm)]
    [InlineData(13, CipherAlgorithm.Aes256Gcm)]
    public void ValidateNonceLength_InvalidLength_ReturnsFalse(int length, CipherAlgorithm algorithm)
    {
        // Act
        var result = _validator.ValidateNonceLength(length, algorithm);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    #endregion

    #region Password Strength Tests

    [Theory]
    [InlineData("weak", PasswordStrength.VeryWeak)]
    [InlineData("password123", PasswordStrength.Weak)]
    [InlineData("Password123", PasswordStrength.Fair)]
    [InlineData("MyStr0ng!Pass", PasswordStrength.Strong)]
    [InlineData("V3ry$tr0ng&P@ssw0rd!2024", PasswordStrength.VeryStrong)]
    public void CalculatePasswordStrength_ReturnsExpectedStrength(string password, PasswordStrength expected)
    {
        // Act
        var result = _validator.CalculatePasswordStrength(password);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Constant Time Comparison Tests

    [Fact]
    public void ConstantTimeEquals_SameArrays_ReturnsTrue()
    {
        // Arrange
        var a = new byte[] { 1, 2, 3, 4, 5 };
        var b = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        var result = _validator.ConstantTimeEquals(a, b);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ConstantTimeEquals_DifferentArrays_ReturnsFalse()
    {
        // Arrange
        var a = new byte[] { 1, 2, 3, 4, 5 };
        var b = new byte[] { 1, 2, 3, 4, 6 };

        // Act
        var result = _validator.ConstantTimeEquals(a, b);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ConstantTimeEquals_DifferentLengths_ReturnsFalse()
    {
        // Arrange
        var a = new byte[] { 1, 2, 3, 4, 5 };
        var b = new byte[] { 1, 2, 3, 4 };

        // Act
        var result = _validator.ConstantTimeEquals(a, b);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ConstantTimeEquals_NullArrays_ReturnsFalse()
    {
        // Act
        var result = _validator.ConstantTimeEquals(null!, null!);

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}
