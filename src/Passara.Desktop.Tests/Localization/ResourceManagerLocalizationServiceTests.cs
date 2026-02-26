using FluentAssertions;
using Passara.Core.Localization;
using Xunit;

namespace Passara.Desktop.Tests.Localization;

public class ResourceManagerLocalizationServiceTests : UnitTestBase
{
    private readonly ResourceManagerLocalizationService _service;

    public ResourceManagerLocalizationServiceTests()
    {
        _service = new ResourceManagerLocalizationService();
    }

    [Fact]
    public void Indexer_WithExistingKey_ReturnsLocalizedString()
    {
        // Act
        var result = _service["App_Name"];

        // Assert
        result.Should().Be("Passara");
    }

    [Fact]
    public void Indexer_WithNonExistingKey_ReturnsKey()
    {
        // Arrange
        const string key = "NonExistingKey";

        // Act
        var result = _service[key];

        // Assert
        result.Should().Be(key);
    }

    [Fact]
    public void Indexer_WithEmptyKey_ReturnsEmptyString()
    {
        // Act
        var result = _service[string.Empty];

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetString_WithExistingKey_ReturnsLocalizedString()
    {
        // Act
        var result = _service.GetString("Button_Save");

        // Assert
        result.Should().Be("Save");
    }

    [Fact]
    public void CurrentCulture_Default_ReturnsCurrentUICulture()
    {
        // Act
        var culture = _service.CurrentCulture;

        // Assert
        culture.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void SetCulture_WithValidCulture_ChangesCurrentCulture()
    {
        // Arrange
        const string newCulture = "cs-CZ";

        // Act
        _service.SetCulture(newCulture);

        // Assert
        _service.CurrentCulture.Should().Be(newCulture);
    }

    [Fact]
    public void SetCulture_WithInvalidCulture_ThrowsCultureNotFoundException()
    {
        // Act
        Action act = () => _service.SetCulture("invalid-culture");

        // Assert
        act.Should().Throw<System.Globalization.CultureNotFoundException>();
    }

    [Fact]
    public void SetCulture_WithEmptyCulture_ThrowsArgumentException()
    {
        // Act
        Action act = () => _service.SetCulture(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Localization_CzechLanguage_ReturnsCzechString()
    {
        // Arrange
        _service.SetCulture("cs-CZ");

        // Act
        var result = _service["Button_Save"];

        // Assert
        result.Should().Be("Uložit");
    }

    [Fact]
    public void Localization_GermanLanguage_ReturnsGermanString()
    {
        // Arrange
        _service.SetCulture("de-DE");

        // Act
        var result = _service["Button_Save"];

        // Assert
        result.Should().Be("Speichern");
    }
}
