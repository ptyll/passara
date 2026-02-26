using System.Security.Cryptography;
using FluentAssertions;
using Passara.Core.Common;
using Passara.Core.Cryptography;
using Xunit;

namespace Passara.Desktop.Tests.Cryptography;

public class MasterKeyDerivationServiceTests : UnitTestBase
{
    private readonly MasterKeyDerivationService _service;

    public MasterKeyDerivationServiceTests()
    {
        _service = new MasterKeyDerivationService();
    }

    #region DeriveFromPassword Tests

    [Fact]
    public async Task DeriveFromPassword_CorrectPassword_ReturnsKey()
    {
        // Arrange
        var password = "correct horse battery staple"u8.ToArray();
        var salt = new byte[KdfParameters.SaltLength];
        RandomNumberGenerator.Fill(salt);

        // Act
        var result = await _service.DeriveFromPasswordAsync(password, salt, KeyDerivationStrength.Interactive);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Length.Should().Be(EncryptionConstants.MasterKeyLength);
    }

    [Fact]
    public async Task DeriveFromPassword_SameInput_ReturnsSameKey()
    {
        // Arrange
        var password = "test password"u8.ToArray();
        var salt = new byte[KdfParameters.SaltLength];
        RandomNumberGenerator.Fill(salt);

        // Act
        var result1 = await _service.DeriveFromPasswordAsync(password, salt, KeyDerivationStrength.Interactive);
        var result2 = await _service.DeriveFromPasswordAsync(password, salt, KeyDerivationStrength.Interactive);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();

        byte[]? key1 = null;
        byte[]? key2 = null;
        result1.Value!.Use(k => { key1 = new byte[k.Length]; Buffer.BlockCopy(k, 0, key1, 0, k.Length); });
        result2.Value!.Use(k => { key2 = new byte[k.Length]; Buffer.BlockCopy(k, 0, key2, 0, k.Length); });

        key1.Should().BeEquivalentTo(key2);
    }

    [Fact]
    public async Task DeriveFromPassword_DifferentSalts_ReturnDifferentKeys()
    {
        // Arrange
        var password = "test password"u8.ToArray();
        var salt1 = new byte[KdfParameters.SaltLength];
        var salt2 = new byte[KdfParameters.SaltLength];
        RandomNumberGenerator.Fill(salt1);
        RandomNumberGenerator.Fill(salt2);

        // Act
        var result1 = await _service.DeriveFromPasswordAsync(password, salt1, KeyDerivationStrength.Interactive);
        var result2 = await _service.DeriveFromPasswordAsync(password, salt2, KeyDerivationStrength.Interactive);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();

        byte[]? key1 = null;
        byte[]? key2 = null;
        result1.Value!.Use(k => { key1 = new byte[k.Length]; Buffer.BlockCopy(k, 0, key1, 0, k.Length); });
        result2.Value!.Use(k => { key2 = new byte[k.Length]; Buffer.BlockCopy(k, 0, key2, 0, k.Length); });

        key1.Should().NotBeEquivalentTo(key2);
    }

    [Fact]
    public async Task DeriveFromPassword_NullPassword_ReturnsFailure()
    {
        // Arrange
        var salt = new byte[KdfParameters.SaltLength];

        // Act
        var result = await _service.DeriveFromPasswordAsync(null!, salt, KeyDerivationStrength.Interactive);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be(ErrorCode.InvalidArgument);
    }

    [Fact]
    public async Task DeriveFromPassword_NullSalt_ReturnsFailure()
    {
        // Arrange
        var password = "test"u8.ToArray();

        // Act
        var result = await _service.DeriveFromPasswordAsync(password, null!, KeyDerivationStrength.Interactive);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be(ErrorCode.InvalidArgument);
    }

    [Fact]
    public async Task DeriveFromPassword_WrongSaltLength_ReturnsFailure()
    {
        // Arrange
        var password = "test"u8.ToArray();
        var salt = new byte[8]; // Wrong length

        // Act
        var result = await _service.DeriveFromPasswordAsync(password, salt, KeyDerivationStrength.Interactive);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be(ErrorCode.InvalidArgument);
    }

    #endregion

    #region Progress Reporting Tests

    [Fact]
    public async Task DeriveFromPassword_WithProgress_ReportsProgress()
    {
        // Arrange
        var password = "test password"u8.ToArray();
        var salt = new byte[KdfParameters.SaltLength];
        RandomNumberGenerator.Fill(salt);
        var progressValues = new List<double>();
        var progress = new Progress<double>(p => progressValues.Add(p));

        // Act
        var result = await _service.DeriveFromPasswordAsync(password, salt, KeyDerivationStrength.Interactive, progress);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    #endregion

    #region Cancellation Tests

    [Fact]
    public async Task DeriveFromPassword_WithCancellation_ThrowsOperationCancelled()
    {
        // Arrange
        var password = "test password"u8.ToArray();
        var salt = new byte[KdfParameters.SaltLength];
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert - TaskCanceledException is a subclass of OperationCanceledException
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await _service.DeriveFromPasswordAsync(password, salt, KeyDerivationStrength.Interactive, cancellationToken: cts.Token);
        });
    }

    #endregion

    #region Different Strength Levels Tests

    [Theory]
    [InlineData(KeyDerivationStrength.Interactive)]
    [InlineData(KeyDerivationStrength.Moderate)]
    [InlineData(KeyDerivationStrength.Sensitive)]
    public async Task DeriveFromPassword_DifferentStrengths_ReturnsKey(KeyDerivationStrength strength)
    {
        // Arrange
        var password = "test password"u8.ToArray();
        var salt = new byte[KdfParameters.SaltLength];
        RandomNumberGenerator.Fill(salt);

        // Act
        var result = await _service.DeriveFromPasswordAsync(password, salt, strength);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    #endregion
}
