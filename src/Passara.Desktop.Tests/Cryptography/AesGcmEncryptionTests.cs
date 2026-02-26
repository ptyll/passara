using System.Security.Cryptography;
using FluentAssertions;
using Passara.Core.Common;
using Passara.Core.Cryptography;
using Xunit;

namespace Passara.Desktop.Tests.Cryptography;

public class AesGcmEncryptionTests : UnitTestBase
{
    #region Encrypt/Decrypt Tests

    [Fact]
    public void EncryptDecrypt_ReturnsOriginal()
    {
        // Arrange
        var cipher = new Aes256GcmCipher();
        var key = new byte[EncryptionConstants.AesKeyLength];
        RandomNumberGenerator.Fill(key);
        var plaintext = "Hello, World!"u8.ToArray();
        var associatedData = "header"u8.ToArray();

        // Act
        var encrypted = cipher.Encrypt(plaintext, key, associatedData);
        var decrypted = cipher.Decrypt(encrypted.Value!, key, associatedData);

        // Assert
        decrypted.IsSuccess.Should().BeTrue();
        decrypted.Value.Should().BeEquivalentTo(plaintext);
    }

    [Fact]
    public void Encrypt_EmptyPlaintext_Works()
    {
        // Arrange
        var cipher = new Aes256GcmCipher();
        var key = new byte[EncryptionConstants.AesKeyLength];
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
        var cipher = new Aes256GcmCipher();
        var key = new byte[EncryptionConstants.AesKeyLength];
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
        var cipher = new Aes256GcmCipher();
        var key1 = new byte[EncryptionConstants.AesKeyLength];
        var key2 = new byte[EncryptionConstants.AesKeyLength];
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
        var cipher = new Aes256GcmCipher();
        var key = new byte[EncryptionConstants.AesKeyLength];
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

    #region Validation Tests

    [Fact]
    public void Encrypt_NullPlaintext_ReturnsFailure()
    {
        // Arrange
        var cipher = new Aes256GcmCipher();
        var key = new byte[EncryptionConstants.AesKeyLength];
        byte[]? plaintext = null;
        var associatedData = "header"u8.ToArray();

        // Act
        var result = cipher.Encrypt(plaintext!, key, associatedData);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be(ErrorCode.InvalidArgument);
    }

    [Fact]
    public void Encrypt_NullKey_ReturnsFailure()
    {
        // Arrange
        var cipher = new Aes256GcmCipher();
        byte[]? key = null;
        var plaintext = "Hello"u8.ToArray();
        var associatedData = "header"u8.ToArray();

        // Act
        var result = cipher.Encrypt(plaintext, key!, associatedData);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be(ErrorCode.InvalidArgument);
    }

    [Fact]
    public void Encrypt_WrongKeyLength_ReturnsFailure()
    {
        // Arrange
        var cipher = new Aes256GcmCipher();
        var key = new byte[16]; // Wrong length
        var plaintext = "Hello"u8.ToArray();
        var associatedData = "header"u8.ToArray();

        // Act
        var result = cipher.Encrypt(plaintext, key, associatedData);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be(ErrorCode.InvalidKey);
    }

    #endregion

    #region Algorithm Tests

    [Fact]
    public void Algorithm_ReturnsAes256Gcm()
    {
        // Arrange
        var cipher = new Aes256GcmCipher();

        // Act
        var algorithm = cipher.Algorithm;

        // Assert
        algorithm.Should().Be(CipherAlgorithm.Aes256Gcm);
    }

    #endregion

    #region EncryptedBlob Tests

    [Fact]
    public void EncryptedBlob_Properties_AreCorrect()
    {
        // Arrange
        var nonce = new byte[] { 1, 2, 3 };
        var ciphertext = new byte[] { 4, 5, 6 };
        var tag = new byte[] { 7, 8, 9 };

        // Act
        var blob = new EncryptedBlob(nonce, ciphertext, tag);

        // Assert
        blob.Nonce.Should().BeEquivalentTo(nonce);
        blob.Ciphertext.Should().BeEquivalentTo(ciphertext);
        blob.Tag.Should().BeEquivalentTo(tag);
    }

    [Fact]
    public void EncryptedBlob_ToByteArray_ReturnsCombinedData()
    {
        // Arrange
        var nonce = new byte[] { 1, 2, 3 };
        var ciphertext = new byte[] { 4, 5, 6 };
        var tag = new byte[] { 7, 8, 9 };
        var blob = new EncryptedBlob(nonce, ciphertext, tag);

        // Act
        var bytes = blob.ToByteArray();

        // Assert
        bytes.Should().HaveCount(nonce.Length + ciphertext.Length + tag.Length);
    }

    [Fact]
    public void EncryptedBlob_FromByteArray_WithCorrectLength_RestoresOriginal()
    {
        // Arrange
        var nonce = new byte[EncryptionConstants.AesNonceLength];
        var ciphertext = new byte[] { 4, 5, 6 };
        var tag = new byte[EncryptionConstants.AesTagLength];
        RandomNumberGenerator.Fill(nonce);
        RandomNumberGenerator.Fill(tag);
        var blob = new EncryptedBlob(nonce, ciphertext, tag);
        var bytes = blob.ToByteArray();

        // Act
        var restored = EncryptedBlob.FromByteArray(bytes, EncryptionConstants.AesNonceLength, EncryptionConstants.AesTagLength);

        // Assert
        restored.Nonce.Should().BeEquivalentTo(nonce);
        restored.Ciphertext.Should().BeEquivalentTo(ciphertext);
        restored.Tag.Should().BeEquivalentTo(tag);
    }

    #endregion
}
