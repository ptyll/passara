namespace Passara.Core.Cryptography;

/// <summary>
/// Defines parameters for key derivation functions.
/// </summary>
public static class KdfParameters
{
    // Argon2id parameters for different security levels
    // Interactive - faster, for mobile devices
    public const int Argon2InteractiveMemoryKib = 65536;      // 64 MB
    public const int Argon2InteractiveIterations = 2;
    public const int Argon2InteractiveParallelism = 1;

    // Moderate - balanced, default for desktop
    public const int Argon2ModerateMemoryKib = 262144;        // 256 MB
    public const int Argon2ModerateIterations = 3;
    public const int Argon2ModerateParallelism = 4;

    // Sensitive - paranoid, slower
    public const int Argon2SensitiveMemoryKib = 1048576;      // 1 GB
    public const int Argon2SensitiveIterations = 4;
    public const int Argon2SensitiveParallelism = 4;

    // Key and salt lengths
    public const int SaltLength = 16;
    public const int KeyLength = 32;

    // PBKDF2 legacy parameters (for import only)
    public const int Pbkdf2DefaultIterations = 600000;
    public const int Pbkdf2MinIterations = 100000;
}
