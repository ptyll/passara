namespace Passara.Core.Cryptography;

/// <summary>
/// Defines the key derivation function (KDF) algorithms supported by the application.
/// </summary>
public enum KdfAlgorithmType
{
    /// <summary>
    /// Argon2id - recommended, modern memory-hard KDF.
    /// </summary>
    Argon2Id = 1,

    /// <summary>
    /// PBKDF2 with SHA-256 - legacy support for import only.
    /// </summary>
    Pbkdf2Sha256 = 2,

    /// <summary>
    /// PBKDF2 with SHA-512 - legacy support for import only.
    /// </summary>
    Pbkdf2Sha512 = 3
}
