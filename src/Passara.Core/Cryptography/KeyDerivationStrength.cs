namespace Passara.Core.Cryptography;

/// <summary>
/// Defines the strength levels for key derivation operations.
/// </summary>
public enum KeyDerivationStrength
{
    /// <summary>
    /// Interactive strength - faster, suitable for mobile devices.
    /// </summary>
    Interactive = 1,

    /// <summary>
    /// Moderate strength - balanced security and performance (default for desktop).
    /// </summary>
    Moderate = 2,

    /// <summary>
    /// Sensitive strength - paranoid security, slower computation.
    /// </summary>
    Sensitive = 3
}
