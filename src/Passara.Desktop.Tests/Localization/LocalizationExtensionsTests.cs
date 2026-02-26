using FluentAssertions;
using Moq;
using Passara.Core.Common;
using Passara.Core.Localization;
using Xunit;

namespace Passara.Desktop.Tests.Localization;

public class LocalizationExtensionsTests : UnitTestBase
{
    private readonly Mock<ILocalizationService> _mockLocalizationService;

    public LocalizationExtensionsTests()
    {
        _mockLocalizationService = new Mock<ILocalizationService>();
    }

    #region L(string key)

    [Fact]
    public void L_WithKey_ReturnsLocalizedString()
    {
        // Arrange
        const string key = "Button_Save";
        const string expected = "Save";
        _mockLocalizationService.Setup(s => s[key]).Returns(expected);

        // Act
        var result = _mockLocalizationService.Object.L(key);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void L_WithNullService_ThrowsArgumentNullException()
    {
        // Arrange
        ILocalizationService? service = null;

        // Act
        Action act = () => service!.L("key");

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void L_WithEmptyKey_ThrowsArgumentException()
    {
        // Act
        Action act = () => _mockLocalizationService.Object.L(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region L(ErrorCode errorCode)

    [Fact]
    public void L_WithErrorCode_ReturnsLocalizedErrorMessage()
    {
        // Arrange
        const string expected = "Invalid master password.";
        _mockLocalizationService.Setup(s => s["Error_InvalidMasterPassword"]).Returns(expected);

        // Act
        var result = _mockLocalizationService.Object.L(ErrorCode.InvalidMasterPassword);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void L_WithDifferentErrorCode_ReturnsCorrectLocalizedError()
    {
        // Arrange
        const string expected = "Vault is locked.";
        _mockLocalizationService.Setup(s => s["Error_VaultLocked"]).Returns(expected);

        // Act
        var result = _mockLocalizationService.Object.L(ErrorCode.VaultLocked);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region L(string key, params object[] args)

    [Fact]
    public void L_WithFormatArgs_ReturnsFormattedString()
    {
        // Arrange
        const string key = "Message_Count";
        _mockLocalizationService.Setup(s => s.GetString(key, It.Is<object[]>(a => a.Length == 1 && (int)a[0] == 5)))
            .Returns("You have 5 items.");

        // Act
        var result = _mockLocalizationService.Object.L(key, 5);

        // Assert
        result.Should().Be("You have 5 items.");
    }

    [Fact]
    public void L_WithMultipleFormatArgs_ReturnsFormattedString()
    {
        // Arrange
        const string key = "Message_UserCount";
        _mockLocalizationService.Setup(s => s.GetString(key, It.Is<object[]>(a => 
            a.Length == 2 && (string)a[0] == "John" && (int)a[1] == 10)))
            .Returns("Hello John, you have 10 messages.");

        // Act
        var result = _mockLocalizationService.Object.L(key, "John", 10);

        // Assert
        result.Should().Be("Hello John, you have 10 messages.");
    }

    [Fact]
    public void L_WithNullServiceAndArgs_ThrowsArgumentNullException()
    {
        // Arrange
        ILocalizationService? service = null;

        // Act
        Action act = () => service!.L("key", "arg");

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion
}
