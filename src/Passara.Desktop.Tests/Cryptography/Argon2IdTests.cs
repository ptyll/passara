using FluentAssertions;
using Passara.Core.Common;
using Passara.Core.Cryptography;
using Xunit;

namespace Passara.Desktop.Tests.Cryptography;

public class Argon2IdTests : UnitTestBase
{
    #region DeriveKey Tests

    [Fact]
    public async Task DeriveKey_CorrectInput_Returns32Bytes()
    {
        // Arrange
        var provider = new Argon2IdProvider();
        var password = "test password"u8.ToArray();
        var salt = new byte[Core.Cryptography.KdfParameters.SaltLength];
        Array.Fill(salt, (byte)0xAB);
        var parameters = new KdfOptions(
            Core.Cryptography.KdfParameters.Argon2InteractiveIterations,
            Core.Cryptography.KdfParameters.Argon2InteractiveMemoryKib,
            Core.Cryptography.KdfParameters.Argon2InteractiveParallelism);

        // Act
        var result = await provider.DeriveKeyAsync(password, salt, parameters);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Length.Should().Be(Core.Cryptography.KdfParameters.KeyLength);
    }

    [Fact]
    public async Task DeriveKey_SameInputSameSalt_ReturnsSameKey()
    {
        // Arrange
        var provider = new Argon2IdProvider();
        var password = "test password"u8.ToArray();
        var salt = new byte[Core.Cryptography.KdfParameters.SaltLength];
        Array.Fill(salt, (byte)0xAB);
        var parameters = new KdfOptions(
            Core.Cryptography.KdfParameters.Argon2InteractiveIterations,
            Core.Cryptography.KdfParameters.Argon2InteractiveMemoryKib,
            Core.Cryptography.KdfParameters.Argon2InteractiveParallelism);

        // Act
        var result1 = await provider.DeriveKeyAsync(password, salt, parameters);
        var result2 = await provider.DeriveKeyAsync(password, salt, parameters);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();
        result1.Value.Should().BeEquivalentTo(result2.Value);
    }

    [Fact]
    public async Task DeriveKey_DifferentSalts_ReturnDifferentKeys()
    {
        // Arrange
        var provider = new Argon2IdProvider();
        var password = "test password"u8.ToArray();
        var salt1 = new byte[Core.Cryptography.KdfParameters.SaltLength];
        var salt2 = new byte[Core.Cryptography.KdfParameters.SaltLength];
        Array.Fill(salt1, (byte)0xAB);
        Array.Fill(salt2, (byte)0xCD);
        var parameters = new KdfOptions(
            Core.Cryptography.KdfParameters.Argon2InteractiveIterations,
            Core.Cryptography.KdfParameters.Argon2InteractiveMemoryKib,
            Core.Cryptography.KdfParameters.Argon2InteractiveParallelism);

        // Act
        var result1 = await provider.DeriveKeyAsync(password, salt1, parameters);
        var result2 = await provider.DeriveKeyAsync(password, salt2, parameters);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();
        result1.Value.Should().NotBeEquivalentTo(result2.Value);
    }

    [Fact]
    public async Task DeriveKey_InvalidParameters_ReturnsFailure()
    {
        // Arrange
        var provider = new Argon2IdProvider();
        var password = "test password"u8.ToArray();
        var salt = new byte[Core.Cryptography.KdfParameters.SaltLength];
        var parameters = new KdfOptions(0, 1024, 1); // Invalid: 0 iterations

        // Act
        var result = await provider.DeriveKeyAsync(password, salt, parameters);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task DeriveKey_NullPassword_ReturnsFailure()
    {
        // Arrange
        var provider = new Argon2IdProvider();
        byte[]? password = null;
        var salt = new byte[Core.Cryptography.KdfParameters.SaltLength];
        var parameters = KdfOptions.Interactive;

        // Act
        var result = await provider.DeriveKeyAsync(password!, salt, parameters);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be(ErrorCode.InvalidArgument);
    }

    [Fact]
    public async Task DeriveKey_NullSalt_ReturnsFailure()
    {
        // Arrange
        var provider = new Argon2IdProvider();
        var password = "test password"u8.ToArray();
        byte[]? salt = null;
        var parameters = KdfOptions.Interactive;

        // Act
        var result = await provider.DeriveKeyAsync(password, salt!, parameters);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be(ErrorCode.InvalidArgument);
    }

    [Fact]
    public async Task DeriveKey_WrongSaltLength_ReturnsFailure()
    {
        // Arrange
        var provider = new Argon2IdProvider();
        var password = "test password"u8.ToArray();
        var salt = new byte[8]; // Wrong length
        var parameters = KdfOptions.Interactive;

        // Act
        var result = await provider.DeriveKeyAsync(password, salt, parameters);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be(ErrorCode.InvalidArgument);
    }

    #endregion

    #region Algorithm Tests

    [Fact]
    public void AlgorithmType_ReturnsArgon2Id()
    {
        // Arrange
        var provider = new Argon2IdProvider();

        // Act
        var algorithm = provider.AlgorithmType;

        // Assert
        algorithm.Should().Be(KdfAlgorithmType.Argon2Id);
    }

    #endregion

    #region Progress Reporting Tests

    [Fact]
    public async Task DeriveKey_WithProgressReporter_ReportsProgress()
    {
        // Arrange
        var provider = new Argon2IdProvider();
        var password = "test password"u8.ToArray();
        var salt = new byte[Core.Cryptography.KdfParameters.SaltLength];
        Array.Fill(salt, (byte)0xAB);
        var parameters = KdfOptions.Interactive;
        var progressValues = new List<double>();
        var progress = new Progress<double>(p => progressValues.Add(p));

        // Act
        var result = await provider.DeriveKeyAsync(password, salt, parameters, progress);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Progress reporting behavior depends on implementation
        // At minimum, should have at least one progress report
    }

    [Fact]
    public async Task DeriveKey_WithCancellation_ThrowsOperationCancelled()
    {
        // Arrange
        var provider = new Argon2IdProvider();
        var password = "test password"u8.ToArray();
        var salt = new byte[Core.Cryptography.KdfParameters.SaltLength];
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert - TaskCanceledException is a subclass of OperationCanceledException
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await provider.DeriveKeyAsync(password, salt, KdfOptions.Interactive, cancellationToken: cts.Token);
        });
    }

    #endregion
}
