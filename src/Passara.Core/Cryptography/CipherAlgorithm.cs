namespace Passara.Core.Cryptography;

/// <summary>
/// Defines the symmetric encryption algorithms supported by the application.
/// </summary>
public enum CipherAlgorithm
{
    /// <summary>
    /// AES-256 in GCM mode - preferred algorithm with hardware acceleration.
    /// </summary>
    Aes256Gcm = 1,

    /// <summary>
    /// ChaCha20-Poly1305 - alternative for mobile and low-power devices.
    /// </summary>
    ChaCha20Poly1305 = 2
}
