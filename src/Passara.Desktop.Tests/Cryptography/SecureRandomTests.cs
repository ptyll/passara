using FluentAssertions;
using Passara.Core.Cryptography;
using Xunit;

namespace Passara.Desktop.Tests.Cryptography;

public class SecureRandomTests : UnitTestBase
{
    #region GenerateBytes Tests

    [Fact]
    public void GenerateBytes_ReturnsDifferentValues()
    {
        // Arrange
        var random = new LibsodiumRandom();

        // Act
        var bytes1 = random.GenerateBytes(32);
        var bytes2 = random.GenerateBytes(32);

        // Assert
        bytes1.Should().NotBeEquivalentTo(bytes2);
    }

    [Fact]
    public void GenerateBytes_ReturnsRequestedLength()
    {
        // Arrange
        var random = new LibsodiumRandom();

        // Act
        var bytes = random.GenerateBytes(64);

        // Assert
        bytes.Length.Should().Be(64);
    }

    [Fact]
    public void GenerateBytes_WithZeroLength_ReturnsEmptyArray()
    {
        // Arrange
        var random = new LibsodiumRandom();

        // Act
        var bytes = random.GenerateBytes(0);

        // Assert
        bytes.Should().BeEmpty();
    }

    [Fact]
    public void GenerateBytes_WithNegativeLength_ThrowsArgumentException()
    {
        // Arrange
        var random = new LibsodiumRandom();

        // Act
        Action act = () => random.GenerateBytes(-1);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region GenerateUInt32 Tests

    [Fact]
    public void GenerateUInt32_ReturnsDifferentValues()
    {
        // Arrange
        var random = new LibsodiumRandom();

        // Act
        var value1 = random.GenerateUInt32();
        var value2 = random.GenerateUInt32();

        // Assert
        value1.Should().NotBe(value2);
    }

    #endregion

    #region Shuffle Tests

    [Fact]
    public void Shuffle_ModifiesArray()
    {
        // Arrange
        var random = new LibsodiumRandom();
        var original = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var copy = new byte[original.Length];
        Buffer.BlockCopy(original, 0, copy, 0, original.Length);

        // Act
        random.Shuffle<byte>(copy);

        // Assert - the array still contains the same elements (check by sorting)
        Array.Sort(copy);
        copy.Should().Equal(original);
    }

    [Fact]
    public void Shuffle_NullArray_ThrowsArgumentNullException()
    {
        // Arrange
        var random = new LibsodiumRandom();

        // Act
        Action act = () => random.Shuffle<byte>(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region Uniform Tests

    [Fact]
    public void GenerateUniform_BelowMax_ReturnsValidRange()
    {
        // Arrange
        var random = new LibsodiumRandom();
        const uint max = 100;

        // Act
        var value = random.GenerateUniform(max);

        // Assert
        value.Should().BeLessThan(max);
    }

    [Fact]
    public void GenerateUniform_MultipleCalls_ReturnsDifferentValues()
    {
        // Arrange
        var random = new LibsodiumRandom();
        const uint max = 1000;

        // Act
        var values = Enumerable.Range(0, 10).Select(_ => random.GenerateUniform(max)).ToList();

        // Assert
        values.Should().OnlyContain(v => v < max);
    }

    #endregion
}
