namespace Passara.Core.Cryptography;

/// <summary>
/// Defines default values for password generation.
/// </summary>
public static class PasswordGenerationDefaults
{
    /// <summary>
    /// Minimum password length.
    /// </summary>
    public const int MinLength = 8;

    /// <summary>
    /// Default password length.
    /// </summary>
    public const int DefaultLength = 16;

    /// <summary>
    /// Maximum password length.
    /// </summary>
    public const int MaxLength = 128;
}
