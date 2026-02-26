using System.Text.RegularExpressions;
using Passara.Core.Common;

namespace Passara.Core.Cryptography;

/// <summary>
/// Validates cryptographic parameters and data.
/// </summary>
public sealed class CryptoValidator : ICryptoValidator
{
    /// <inheritdoc />
    public Result ValidateKeyLength(int length)
    {
        // Valid AES key lengths: 128, 192, 256 bits (16, 24, 32 bytes)
        if (length != 16 && length != 24 && length != 32)
        {
            return Result.Failure(ErrorCode.InvalidKey, "Key length must be 16, 24, or 32 bytes.");
        }

        return Result.Success();
    }

    /// <inheritdoc />
    public Result ValidateSaltLength(int length)
    {
        if (length != KdfParameters.SaltLength)
        {
            return Result.Failure(ErrorCode.InvalidArgument, $"Salt length must be exactly {KdfParameters.SaltLength} bytes.");
        }

        return Result.Success();
    }

    /// <inheritdoc />
    public Result ValidateNonceLength(int length, CipherAlgorithm algorithm)
    {
        var expectedLength = algorithm switch
        {
            CipherAlgorithm.Aes256Gcm => EncryptionConstants.AesNonceLength,
            CipherAlgorithm.ChaCha20Poly1305 => EncryptionConstants.ChaChaNonceLength,
            _ => -1
        };

        if (expectedLength == -1)
        {
            return Result.Failure(ErrorCode.InvalidArgument, "Unknown cipher algorithm.");
        }

        if (length != expectedLength)
        {
            return Result.Failure(ErrorCode.InvalidArgument, $"Nonce length must be exactly {expectedLength} bytes for {algorithm}.");
        }

        return Result.Success();
    }

    /// <inheritdoc />
    public PasswordStrength CalculatePasswordStrength(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return PasswordStrength.VeryWeak;
        }

        // Calculate pool size based on character types
        int poolSize = 0;
        if (Regex.IsMatch(password, "[a-z]"))
        {
            poolSize += 26;
        }
        if (Regex.IsMatch(password, "[A-Z]"))
        {
            poolSize += 26;
        }
        if (Regex.IsMatch(password, "[0-9]"))
        {
            poolSize += 10;
        }
        if (Regex.IsMatch(password, "[^a-zA-Z0-9]"))
        {
            poolSize += 32;
        }

        if (poolSize == 0)
        {
            return PasswordStrength.VeryWeak;
        }

        // Calculate entropy
        double entropy = password.Length * Math.Log(poolSize) / Math.Log(2);

        return entropy switch
        {
            < 40 => PasswordStrength.VeryWeak,
            < 60 => PasswordStrength.Weak,
            < 80 => PasswordStrength.Fair,
            < 120 => PasswordStrength.Strong,
            _ => PasswordStrength.VeryStrong
        };
    }

    /// <inheritdoc />
    public bool ConstantTimeEquals(byte[] a, byte[] b)
    {
        if (a == null || b == null)
        {
            return false;
        }

        if (a.Length != b.Length)
        {
            return false;
        }

        // Use CryptographicOperations.FixedTimeEquals if available (.NET Core 2.1+)
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(a, b);
#else
        // Manual constant-time comparison
        int result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }
        return result == 0;
#endif
    }
}
