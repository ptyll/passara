namespace Passara.Core.Cryptography;

/// <summary>
/// Defines cryptographic constants for encryption operations.
/// </summary>
public static class EncryptionConstants
{
    // AES-GCM constants
    public const int AesKeyLength = 32;
    public const int AesNonceLength = 12;
    public const int AesTagLength = 16;

    // ChaCha20-Poly1305 constants (original variant uses 8-byte nonce)
    public const int ChaChaKeyLength = 32;
    public const int ChaChaNonceLength = 8;

    // Master key length
    public const int MasterKeyLength = 32;
}
