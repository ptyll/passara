using FluentAssertions;
using Passara.Core.Cryptography;
using Xunit;

namespace Passara.Desktop.Tests.Cryptography;

public class SecureBufferTests : UnitTestBase
{
    #region Allocation Tests

    [Fact]
    public void Constructor_WithValidSize_AllocatesBuffer()
    {
        // Arrange
        const int size = 32;

        // Act
        using var buffer = new SecureBuffer(size);

        // Assert
        buffer.Size.Should().Be(size);
        buffer.IsDisposed.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithZeroSize_AllocatesMinimumBuffer()
    {
        // Act
        using var buffer = new SecureBuffer(0);

        // Assert
        buffer.Size.Should().BeGreaterOrEqualTo(0);
        buffer.IsDisposed.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithNegativeSize_ThrowsArgumentException()
    {
        // Act
        Action act = () => new SecureBuffer(-1);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithExcessiveSize_ThrowsArgumentException()
    {
        // Act - try to allocate more than reasonable (1GB)
        Action act = () => new SecureBuffer(1024 * 1024 * 1024 + 1);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Data Access Tests

    [Fact]
    public void UseRead_StoresData_CanReadBack()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var buffer = new SecureBuffer(data.Length);
        buffer.UseWrite(bytes => Buffer.BlockCopy(data, 0, bytes, 0, data.Length));

        // Act
        byte[]? readData = null;
        buffer.UseRead(bytes =>
        {
            readData = new byte[bytes.Length];
            Buffer.BlockCopy(bytes, 0, readData, 0, bytes.Length);
        });

        // Assert
        readData.Should().BeEquivalentTo(data);
    }

    [Fact]
    public void UseRead_WithNullAction_ThrowsArgumentNullException()
    {
        // Arrange
        using var buffer = new SecureBuffer(32);

        // Act
        Action act = () => buffer.UseRead((Action<byte[]>)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void UseWrite_WithNullAction_ThrowsArgumentNullException()
    {
        // Arrange
        using var buffer = new SecureBuffer(32);

        // Act
        Action act = () => buffer.UseWrite(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void UseRead_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var buffer = new SecureBuffer(32);
        buffer.Dispose();

        // Act
        Action act = () => buffer.UseRead(_ => { });

        // Assert
        act.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void UseWrite_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var buffer = new SecureBuffer(32);
        buffer.Dispose();

        // Act
        Action act = () => buffer.UseWrite(_ => { });

        // Assert
        act.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void UseRead_WithFunc_ReturnsResult()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };
        using var buffer = new SecureBuffer(data.Length);
        buffer.UseWrite(bytes => Buffer.BlockCopy(data, 0, bytes, 0, data.Length));

        // Act
        var result = buffer.UseRead(bytes => bytes[0] + bytes[1] + bytes[2]);

        // Assert
        result.Should().Be(6);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_SetsIsDisposedFlag()
    {
        // Arrange
        var buffer = new SecureBuffer(32);

        // Act
        buffer.Dispose();

        // Assert
        buffer.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var buffer = new SecureBuffer(32);

        // Act
        buffer.Dispose();
        buffer.Dispose();

        // Assert
        buffer.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void Dispose_AfterDisposal_SizeStillAccessible()
    {
        // Arrange
        var buffer = new SecureBuffer(32);

        // Act
        buffer.Dispose();

        // Assert
        buffer.Size.Should().Be(32);
    }

    #endregion

    #region ISensitiveData Compliance

    [Fact]
    public void SecureBuffer_ImplementsISecureBuffer()
    {
        // Arrange & Act
        using var buffer = new SecureBuffer(32);

        // Assert
        buffer.Should().BeAssignableTo<ISecureBuffer>();
    }

    [Fact]
    public void SecureBuffer_ImplementsIDisposable()
    {
        // Arrange & Act
        using var buffer = new SecureBuffer(32);

        // Assert
        buffer.Should().BeAssignableTo<IDisposable>();
    }

    #endregion

    #region Security Tests

    [Fact]
    public void UseRead_DoesNotExposeInternalBuffer()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };
        using var buffer = new SecureBuffer(data.Length);
        buffer.UseWrite(bytes => Buffer.BlockCopy(data, 0, bytes, 0, data.Length));

        byte[]? firstReference = null;
        byte[]? secondReference = null;

        // Act
        buffer.UseRead(bytes => firstReference = bytes);
        buffer.UseRead(bytes => secondReference = bytes);

        // Assert - the arrays should be different instances (copies)
        firstReference.Should().NotBeSameAs(secondReference);
    }

    [Fact]
    public void SensitiveData_Generic_CreatesTypedWrapper()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };

        // Act
        using var sensitive = SensitiveData<byte[]>.Create(data, bytes => bytes.Length);

        // Assert
        sensitive.Value.Should().BeEquivalentTo(data);
    }

    [Fact]
    public void SensitiveData_Dispose_CallsCleanupAction()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };
        var cleanupCalled = false;

        // Act
        using (var sensitive = SensitiveData<byte[]>.Create(
            data,
            bytes => bytes.Length,
            _ => cleanupCalled = true))
        {
            // Just use it
        }

        // Assert
        cleanupCalled.Should().BeTrue();
    }

    #endregion
}
