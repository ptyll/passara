namespace Passara.Core.Cryptography;

/// <summary>
/// Defines character sets for password generation.
/// </summary>
[Flags]
public enum PasswordCharacterSet
{
    /// <summary>
    /// No character set selected.
    /// </summary>
    None = 0,

    /// <summary>
    /// Uppercase letters A-Z.
    /// </summary>
    Uppercase = 1,

    /// <summary>
    /// Lowercase letters a-z.
    /// </summary>
    Lowercase = 2,

    /// <summary>
    /// Digits 0-9.
    /// </summary>
    Digits = 4,

    /// <summary>
    /// Special characters.
    /// </summary>
    Special = 8,

    /// <summary>
    /// All character sets combined.
    /// </summary>
    All = Uppercase | Lowercase | Digits | Special
}
