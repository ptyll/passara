using FluentAssertions;
using Passara.Core.Cryptography;
using Xunit;

namespace Passara.Desktop.Tests.Cryptography;

public class PasswordGeneratorTests : UnitTestBase
{
    private readonly PasswordGenerator _generator;

    public PasswordGeneratorTests()
    {
        _generator = new PasswordGenerator(new LibsodiumRandom());
    }

    #region Length Tests

    [Fact]
    public void Generate_LengthCorrect()
    {
        // Arrange
        const int length = 20;

        // Act
        var password = _generator.Generate(length, PasswordCharacterSet.All);

        // Assert
        password.Length.Should().Be(length);
    }

    [Fact]
    public void Generate_BelowMinLength_ThrowsArgumentException()
    {
        // Act
        Action act = () => _generator.Generate(PasswordGenerationDefaults.MinLength - 1, PasswordCharacterSet.All);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Generate_AboveMaxLength_ThrowsArgumentException()
    {
        // Act
        Action act = () => _generator.Generate(PasswordGenerationDefaults.MaxLength + 1, PasswordCharacterSet.All);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Character Set Tests

    [Fact]
    public void Generate_WithUppercase_ContainsUppercase()
    {
        // Act
        var password = _generator.Generate(20, PasswordCharacterSet.Uppercase);

        // Assert
        password.Should().MatchRegex("[A-Z]");
    }

    [Fact]
    public void Generate_WithLowercase_ContainsLowercase()
    {
        // Act
        var password = _generator.Generate(20, PasswordCharacterSet.Lowercase);

        // Assert
        password.Should().MatchRegex("[a-z]");
    }

    [Fact]
    public void Generate_WithDigits_ContainsDigits()
    {
        // Act
        var password = _generator.Generate(20, PasswordCharacterSet.Digits);

        // Assert
        password.Should().MatchRegex("[0-9]");
    }

    [Fact]
    public void Generate_WithSpecial_ContainsSpecial()
    {
        // Act
        var password = _generator.Generate(20, PasswordCharacterSet.Special);

        // Assert
        password.Should().MatchRegex("[!@#$%^&*()_+\\-=\\[\\]{}|;:,.<>?]");
    }

    [Fact]
    public void Generate_WithAll_ContainsAllTypes()
    {
        // Act
        var password = _generator.Generate(20, PasswordCharacterSet.All);

        // Assert
        password.Should().MatchRegex("[A-Z]");
        password.Should().MatchRegex("[a-z]");
        password.Should().MatchRegex("[0-9]");
        password.Should().MatchRegex("[!@#$%^&*()_+\\-=\\[\\]{}|;:,.<>?]");
    }

    [Fact]
    public void Generate_WithNone_ThrowsArgumentException()
    {
        // Act
        Action act = () => _generator.Generate(20, PasswordCharacterSet.None);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Require All Types Tests

    [Fact]
    public void Generate_RequireAllTypes_ContainsAllTypes()
    {
        // Act
        var password = _generator.Generate(20, PasswordCharacterSet.All, requireAllTypes: true);

        // Assert
        password.Should().MatchRegex("[A-Z]");
        password.Should().MatchRegex("[a-z]");
        password.Should().MatchRegex("[0-9]");
        password.Should().MatchRegex("[!@#$%^&*()_+\\-=\\[\\]{}|;:,.<>?]");
    }

    [Fact]
    public void Generate_RequireAllTypesWithInsufficientLength_ThrowsArgumentException()
    {
        // Act - need at least 4 chars to have one of each type
        Action act = () => _generator.Generate(3, PasswordCharacterSet.All, requireAllTypes: true);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Randomness Tests

    [Fact]
    public void Generate_MultipleCalls_ReturnsDifferentPasswords()
    {
        // Act
        var passwords = Enumerable.Range(0, 10)
            .Select(_ => _generator.Generate(20, PasswordCharacterSet.All))
            .ToList();

        // Assert
        passwords.Should().OnlyHaveUniqueItems();
    }

    #endregion

    #region CalculateEntropy Tests

    [Theory]
    [InlineData(8, PasswordCharacterSet.Lowercase, PasswordStrength.VeryWeak)] // 8 * log2(26) = 37.6 bits
    [InlineData(12, PasswordCharacterSet.Lowercase, PasswordStrength.Weak)] // 12 * log2(26) = 56.4 bits
    [InlineData(16, PasswordCharacterSet.Lowercase | PasswordCharacterSet.Uppercase, PasswordStrength.Strong)] // 16 * log2(52) = 91.2 bits
    [InlineData(12, PasswordCharacterSet.All, PasswordStrength.Fair)] // 12 * log2(94) = 78.6 bits
    [InlineData(20, PasswordCharacterSet.All, PasswordStrength.VeryStrong)] // 20 * log2(94) = 131.1 bits
    public void CalculateEntropy_ReturnsExpectedStrength(int length, PasswordCharacterSet charSet, PasswordStrength expected)
    {
        // Act
        var strength = PasswordGenerator.CalculateStrength(length, charSet);

        // Assert
        strength.Should().Be(expected);
    }

    #endregion

    #region Character Set Constants Tests

    [Fact]
    public void UppercaseCharset_Contains26Characters()
    {
        PasswordGenerator.UppercaseCharset.Length.Should().Be(26);
    }

    [Fact]
    public void LowercaseCharset_Contains26Characters()
    {
        PasswordGenerator.LowercaseCharset.Length.Should().Be(26);
    }

    [Fact]
    public void DigitsCharset_Contains10Characters()
    {
        PasswordGenerator.DigitsCharset.Length.Should().Be(10);
    }

    [Fact]
    public void SpecialCharset_Contains32Characters()
    {
        PasswordGenerator.SpecialCharset.Length.Should().Be(32);
    }

    #endregion
}
