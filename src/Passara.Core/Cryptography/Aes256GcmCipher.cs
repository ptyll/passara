using System.Security.Cryptography;
using Passara.Core.Common;

namespace Passara.Core.Cryptography;

/// <summary>
/// Provides AES-256-GCM encryption using .NET's built-in implementation.
/// </summary>
public sealed class Aes256GcmCipher : ISymmetricCipher
{
    /// <inheritdoc />
    public CipherAlgorithm Algorithm => CipherAlgorithm.Aes256Gcm;

    /// <inheritdoc />
    public Result<EncryptedBlob> Encrypt(byte[] plaintext, byte[] key, byte[]? associatedData = null)
    {
        // Validate inputs
        if (plaintext == null)
        {
            return Result<EncryptedBlob>.Failure(ErrorCode.InvalidArgument, "Plaintext cannot be null.");
        }

        if (key == null)
        {
            return Result<EncryptedBlob>.Failure(ErrorCode.InvalidArgument, "Key cannot be null.");
        }

        if (key.Length != EncryptionConstants.AesKeyLength)
        {
            return Result<EncryptedBlob>.Failure(ErrorCode.InvalidKey, $"Key must be exactly {EncryptionConstants.AesKeyLength} bytes for AES-256.");
        }

        try
        {
            // Generate a random nonce
            var nonce = new byte[EncryptionConstants.AesNonceLength];
            RandomNumberGenerator.Fill(nonce);

            // Create the AES-GCM instance
#if NET5_0_OR_GREATER
            using var aesGcm = new AesGcm(key, EncryptionConstants.AesTagLength);
#else
            using var aesGcm = new AesGcm(key);
#endif

            // Allocate buffers
            var ciphertext = new byte[plaintext.Length];
            var tag = new byte[EncryptionConstants.AesTagLength];

            // Perform encryption
            aesGcm.Encrypt(nonce, plaintext, ciphertext, tag, associatedData);

            return Result<EncryptedBlob>.Success(new EncryptedBlob(nonce, ciphertext, tag));
        }
        catch (PlatformNotSupportedException)
        {
            return Result<EncryptedBlob>.Failure(ErrorCode.EncryptionFailed, "AES-GCM is not supported on this platform.");
        }
        catch (CryptographicException ex)
        {
            return Result<EncryptedBlob>.Failure(ErrorCode.EncryptionFailed, $"Encryption failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public Result<byte[]> Decrypt(EncryptedBlob encryptedBlob, byte[] key, byte[]? associatedData = null)
    {
        // Validate inputs
        if (encryptedBlob == null)
        {
            return Result<byte[]>.Failure(ErrorCode.InvalidArgument, "Encrypted blob cannot be null.");
        }

        if (key == null)
        {
            return Result<byte[]>.Failure(ErrorCode.InvalidArgument, "Key cannot be null.");
        }

        if (key.Length != EncryptionConstants.AesKeyLength)
        {
            return Result<byte[]>.Failure(ErrorCode.InvalidKey, $"Key must be exactly {EncryptionConstants.AesKeyLength} bytes for AES-256.");
        }

        if (encryptedBlob.Nonce.Length != EncryptionConstants.AesNonceLength)
        {
            return Result<byte[]>.Failure(ErrorCode.InvalidArgument, $"Nonce must be exactly {EncryptionConstants.AesNonceLength} bytes.");
        }

        if (encryptedBlob.Tag.Length != EncryptionConstants.AesTagLength)
        {
            return Result<byte[]>.Failure(ErrorCode.InvalidArgument, $"Tag must be exactly {EncryptionConstants.AesTagLength} bytes.");
        }

        try
        {
            // Create the AES-GCM instance
#if NET5_0_OR_GREATER
            using var aesGcm = new AesGcm(key, EncryptionConstants.AesTagLength);
#else
            using var aesGcm = new AesGcm(key);
#endif

            // Allocate buffer for plaintext
            var plaintext = new byte[encryptedBlob.Ciphertext.Length];

            // Perform decryption
            aesGcm.Decrypt(encryptedBlob.Nonce, encryptedBlob.Ciphertext, encryptedBlob.Tag, plaintext, associatedData);

            return Result<byte[]>.Success(plaintext);
        }
        catch (PlatformNotSupportedException)
        {
            return Result<byte[]>.Failure(ErrorCode.DecryptionFailed, "AES-GCM is not supported on this platform.");
        }
        catch (CryptographicException)
        {
            // Authentication failed - wrong key, tampered data, or wrong associated data
            return Result<byte[]>.Failure(ErrorCode.DecryptionFailed, "Decryption failed. The key may be incorrect or the data may have been tampered with.");
        }
    }
}
