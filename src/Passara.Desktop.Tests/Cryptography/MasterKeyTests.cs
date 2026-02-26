using System.Security.Cryptography;
using FluentAssertions;
using Passara.Core.Common;
using Passara.Core.Cryptography;
using Xunit;

namespace Passara.Desktop.Tests.Cryptography;

public class MasterKeyTests : UnitTestBase
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidKey_StoresKey()
    {
        // Arrange
        var key = new byte[EncryptionConstants.MasterKeyLength];
        RandomNumberGenerator.Fill(key);

        // Act
        using var masterKey = new MasterKey(key);

        // Assert
        masterKey.IsDisposed.Should().BeFalse();
        masterKey.Length.Should().Be(EncryptionConstants.MasterKeyLength);
    }

    [Fact]
    public void Constructor_WithNullKey_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => new MasterKey(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithWrongLength_ThrowsArgumentException()
    {
        // Arrange
        var key = new byte[16]; // Wrong length

        // Act
        Action act = () => new MasterKey(key);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Use Tests

    [Fact]
    public void Use_CanAccessKeyData()
    {
        // Arrange
        var key = new byte[EncryptionConstants.MasterKeyLength];
        RandomNumberGenerator.Fill(key);
        using var masterKey = new MasterKey(key);

        // Act
        byte[]? accessedData = null;
        masterKey.Use(k =>
        {
            accessedData = new byte[k.Length];
            Buffer.BlockCopy(k, 0, accessedData, 0, k.Length);
        });

        // Assert
        accessedData.Should().BeEquivalentTo(key);
    }

    [Fact]
    public void Use_WithFunc_ReturnsResult()
    {
        // Arrange
        var key = new byte[EncryptionConstants.MasterKeyLength];
        RandomNumberGenerator.Fill(key);
        using var masterKey = new MasterKey(key);

        // Act
        var result = masterKey.Use(k => k.Length);

        // Assert
        result.Should().Be(EncryptionConstants.MasterKeyLength);
    }

    [Fact]
    public void Use_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        var key = new byte[EncryptionConstants.MasterKeyLength];
        RandomNumberGenerator.Fill(key);
        var masterKey = new MasterKey(key);
        masterKey.Dispose();

        // Act
        Action act = () => masterKey.Use(_ => { });

        // Assert
        act.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void Use_NullAction_ThrowsArgumentNullException()
    {
        // Arrange
        var key = new byte[EncryptionConstants.MasterKeyLength];
        RandomNumberGenerator.Fill(key);
        using var masterKey = new MasterKey(key);

        // Act
        Action act = () => masterKey.Use((Action<byte[]>)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_SetsIsDisposed()
    {
        // Arrange
        var key = new byte[EncryptionConstants.MasterKeyLength];
        RandomNumberGenerator.Fill(key);
        var masterKey = new MasterKey(key);

        // Act
        masterKey.Dispose();

        // Assert
        masterKey.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var key = new byte[EncryptionConstants.MasterKeyLength];
        RandomNumberGenerator.Fill(key);
        var masterKey = new MasterKey(key);

        // Act
        masterKey.Dispose();
        masterKey.Dispose();

        // Assert
        masterKey.IsDisposed.Should().BeTrue();
    }

    #endregion

    #region CreateRandom Tests

    [Fact]
    public void CreateRandom_GeneratesKey()
    {
        // Act
        using var masterKey = MasterKey.CreateRandom();

        // Assert
        masterKey.Should().NotBeNull();
        masterKey.Length.Should().Be(EncryptionConstants.MasterKeyLength);
    }

    [Fact]
    public void CreateRandom_MultipleCalls_ReturnDifferentKeys()
    {
        // Act
        using var key1 = MasterKey.CreateRandom();
        using var key2 = MasterKey.CreateRandom();

        byte[]? data1 = null;
        byte[]? data2 = null;
        key1.Use(k => { data1 = new byte[k.Length]; Buffer.BlockCopy(k, 0, data1, 0, k.Length); });
        key2.Use(k => { data2 = new byte[k.Length]; Buffer.BlockCopy(k, 0, data2, 0, k.Length); });

        // Assert
        data1.Should().NotBeEquivalentTo(data2);
    }

    #endregion
}
