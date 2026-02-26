using System.Security.Cryptography;
using FluentAssertions;
using Passara.Core.Common;
using Passara.Core.Cryptography;
using Xunit;

namespace Passara.Desktop.Tests.Cryptography;

public class ChaCha20Poly1305Tests : UnitTestBase
{
    #region Encrypt/Decrypt Tests

    [Fact]
    public void EncryptDecrypt_ReturnsOriginal()
    {
        // Arrange
        var cipher = new ChaCha20Poly1305Cipher();
        var key = new byte[EncryptionConstants.ChaChaKeyLength];
        RandomNumberGenerator.Fill(key);
        var plaintext = "Hello, World!"u8.ToArray();
        var associatedData = "header"u8.ToArray();

        // Act
        var encrypted = cipher.Encrypt(plaintext, key, associatedData);
        encrypted.IsSuccess.Should().BeTrue($"Encryption failed: {encrypted.ErrorMessage}");
        
        var decrypted = cipher.Decrypt(encrypted.Value!, key, associatedData);

        // Assert
        decrypted.IsSuccess.Should().BeTrue($"Decryption failed: {decrypted.ErrorMessage}");
        decrypted.Value.Should().BeEquivalentTo(plaintext);
    }

    [Fact]
    public void Encrypt_EmptyPlaintext_Works()
    {
        // Arrange
        var cipher = new ChaCha20Poly1305Cipher();
        var key = new byte[EncryptionConstants.ChaChaKeyLength];
        RandomNumberGenerator.Fill(key);
        var plaintext = Array.Empty<byte>();
        var associatedData = "header"u8.ToArray();

        // Act
        var encrypted = cipher.Encrypt(plaintext, key, associatedData);
        var decrypted = cipher.Decrypt(encrypted.Value!, key, associatedData);

        // Assert
        decrypted.IsSuccess.Should().BeTrue();
        decrypted.Value.Should().BeEquivalentTo(plaintext);
    }

    [Fact]
    public void Decrypt_TamperedData_ReturnsFailure()
    {
        // Arrange
        var cipher = new ChaCha20Poly1305Cipher();
        var key = new byte[EncryptionConstants.ChaChaKeyLength];
        RandomNumberGenerator.Fill(key);
        var plaintext = "Hello, World!"u8.ToArray();
        var associatedData = "header"u8.ToArray();

        var encrypted = cipher.Encrypt(plaintext, key, associatedData);

        // Tamper with the ciphertext
        encrypted.Value!.Ciphertext[0] ^= 0xFF;

        // Act
        var decrypted = cipher.Decrypt(encrypted.Value!, key, associatedData);

        // Assert
        decrypted.IsFailure.Should().BeTrue();
        decrypted.ErrorCode.Should().Be(ErrorCode.DecryptionFailed);
    }

    [Fact]
    public void Decrypt_WrongKey_ReturnsFailure()
    {
        // Arrange
        var cipher = new ChaCha20Poly1305Cipher();
        var key1 = new byte[EncryptionConstants.ChaChaKeyLength];
        var key2 = new byte[EncryptionConstants.ChaChaKeyLength];
        RandomNumberGenerator.Fill(key1);
        RandomNumberGenerator.Fill(key2);
        var plaintext = "Hello, World!"u8.ToArray();
        var associatedData = "header"u8.ToArray();

        var encrypted = cipher.Encrypt(plaintext, key1, associatedData);

        // Act
        var decrypted = cipher.Decrypt(encrypted.Value!, key2, associatedData);

        // Assert
        decrypted.IsFailure.Should().BeTrue();
        decrypted.ErrorCode.Should().Be(ErrorCode.DecryptionFailed);
    }

    [Fact]
    public void Decrypt_WrongAssociatedData_ReturnsFailure()
    {
        // Arrange
        var cipher = new ChaCha20Poly1305Cipher();
        var key = new byte[EncryptionConstants.ChaChaKeyLength];
        RandomNumberGenerator.Fill(key);
        var plaintext = "Hello, World!"u8.ToArray();
        var associatedData1 = "header1"u8.ToArray();
        var associatedData2 = "header2"u8.ToArray();

        var encrypted = cipher.Encrypt(plaintext, key, associatedData1);

        // Act
        var decrypted = cipher.Decrypt(encrypted.Value!, key, associatedData2);

        // Assert
        decrypted.IsFailure.Should().BeTrue();
        decrypted.ErrorCode.Should().Be(ErrorCode.DecryptionFailed);
    }

    #endregion

    #region Algorithm Tests

    [Fact]
    public void Algorithm_ReturnsChaCha20Poly1305()
    {
        // Arrange
        var cipher = new ChaCha20Poly1305Cipher();

        // Act
        var algorithm = cipher.Algorithm;

        // Assert
        algorithm.Should().Be(CipherAlgorithm.ChaCha20Poly1305);
    }

    #endregion
}
