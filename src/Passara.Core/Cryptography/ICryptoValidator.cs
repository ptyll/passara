using Passara.Core.Common;

namespace Passara.Core.Cryptography;

/// <summary>
/// Validates cryptographic parameters and data.
/// </summary>
public interface ICryptoValidator
{
    /// <summary>
    /// Validates that the key length is valid for symmetric encryption.
    /// </summary>
    /// <param name="length">The key length in bytes.</param>
    /// <returns>A result indicating success or failure.</returns>
    Result ValidateKeyLength(int length);

    /// <summary>
    /// Validates that the salt length is valid.
    /// </summary>
    /// <param name="length">The salt length in bytes.</param>
    /// <returns>A result indicating success or failure.</returns>
    Result ValidateSaltLength(int length);

    /// <summary>
    /// Validates that the nonce length is valid for the specified algorithm.
    /// </summary>
    /// <param name="length">The nonce length in bytes.</param>
    /// <param name="algorithm">The cipher algorithm.</param>
    /// <returns>A result indicating success or failure.</returns>
    Result ValidateNonceLength(int length, CipherAlgorithm algorithm);

    /// <summary>
    /// Calculates the strength of a password.
    /// </summary>
    /// <param name="password">The password to evaluate.</param>
    /// <returns>The estimated password strength.</returns>
    PasswordStrength CalculatePasswordStrength(string password);

    /// <summary>
    /// Compares two byte arrays in constant time to prevent timing attacks.
    /// </summary>
    /// <param name="a">The first array.</param>
    /// <param name="b">The second array.</param>
    /// <returns>True if the arrays are equal; otherwise, false.</returns>
    bool ConstantTimeEquals(byte[] a, byte[] b);
}
