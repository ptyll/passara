using FluentAssertions;
using Passara.Core.Common;
using Xunit;

namespace Passara.Desktop.Tests.Common;

public class SecureMemoryTests : UnitTestBase
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithByteArray_StoresData()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        using var secureMemory = new SecureMemory(data);

        // Assert
        secureMemory.Length.Should().Be(data.Length);
    }

    [Fact]
    public void Constructor_WithNullByteArray_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => new SecureMemory((byte[])null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithString_StoresData()
    {
        // Arrange
        const string data = "test secret";

        // Act
        using var secureMemory = new SecureMemory(data);

        // Assert
        secureMemory.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Constructor_WithNullString_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => new SecureMemory((string)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region Use Tests

    [Fact]
    public void Use_WithAction_ExecutesWithDataCopy()
    {
        // Arrange
        var originalData = new byte[] { 1, 2, 3, 4, 5 };
        using var secureMemory = new SecureMemory(originalData);
        byte[]? capturedData = null;

        // Act
        secureMemory.Use(data =>
        {
            capturedData = new byte[data.Length];
            Buffer.BlockCopy(data, 0, capturedData, 0, data.Length);
        });

        // Assert
        capturedData.Should().BeEquivalentTo(originalData);
    }

    [Fact]
    public void Use_WithFunc_ReturnsResult()
    {
        // Arrange
        var originalData = new byte[] { 1, 2, 3 };
        using var secureMemory = new SecureMemory(originalData);

        // Act
        var result = secureMemory.Use(data => data.Length);

        // Assert
        result.Should().Be(originalData.Length);
    }

    [Fact]
    public void Use_WithNullAction_ThrowsArgumentNullException()
    {
        // Arrange
        using var secureMemory = new SecureMemory(new byte[] { 1 });

        // Act
        Action act = () => secureMemory.Use((Action<byte[]>)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Use_WithNullFunc_ThrowsArgumentNullException()
    {
        // Arrange
        using var secureMemory = new SecureMemory(new byte[] { 1 });

        // Act
        Action act = () => secureMemory.Use((Func<byte[], int>)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Use_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var secureMemory = new SecureMemory(new byte[] { 1 });
        secureMemory.Dispose();

        // Act
        Action act = () => secureMemory.Use(_ => { });

        // Assert
        act.Should().Throw<ObjectDisposedException>();
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_WithStringData_ReturnsOriginalString()
    {
        // Arrange
        const string original = "test secret";
        using var secureMemory = new SecureMemory(original);

        // Act
        var result = secureMemory.ToString();

        // Assert
        result.Should().Be(original);
    }

    [Fact]
    public void ToString_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var secureMemory = new SecureMemory("test");
        secureMemory.Dispose();

        // Act
        Action act = () => secureMemory.ToString();

        // Assert
        act.Should().Throw<ObjectDisposedException>();
    }

    #endregion

    #region Length Tests

    [Fact]
    public void Length_ReturnsCorrectValue()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var secureMemory = new SecureMemory(data);

        // Act
        var length = secureMemory.Length;

        // Assert
        length.Should().Be(5);
    }

    [Fact]
    public void Length_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var secureMemory = new SecureMemory(new byte[] { 1 });
        secureMemory.Dispose();

        // Act
        Action act = () => _ = secureMemory.Length;

        // Assert
        act.Should().Throw<ObjectDisposedException>();
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_SetsIsDisposedFlag()
    {
        // Arrange
        var secureMemory = new SecureMemory(new byte[] { 1 });

        // Act
        secureMemory.Dispose();

        // Assert
        secureMemory.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var secureMemory = new SecureMemory(new byte[] { 1 });

        // Act
        secureMemory.Dispose();
        secureMemory.Dispose();

        // Assert
        secureMemory.IsDisposed.Should().BeTrue();
    }

    #endregion

    #region Extension Method Tests

    [Fact]
    public void ToSecureMemory_String_ReturnsSecureMemory()
    {
        // Arrange
        const string value = "secret";

        // Act
        using var secureMemory = value.ToSecureMemory();

        // Assert
        secureMemory.Should().NotBeNull();
        secureMemory.ToString().Should().Be(value);
    }

    [Fact]
    public void ToSecureMemory_String_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        string? value = null;

        // Act
        Action act = () => value!.ToSecureMemory();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToSecureMemory_ByteArray_ReturnsSecureMemory()
    {
        // Arrange
        var value = new byte[] { 1, 2, 3 };

        // Act
        using var secureMemory = value.ToSecureMemory();

        // Assert
        secureMemory.Should().NotBeNull();
        secureMemory.Length.Should().Be(value.Length);
    }

    [Fact]
    public void ToSecureMemory_ByteArray_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        byte[]? value = null;

        // Act
        Action act = () => value!.ToSecureMemory();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion
}
