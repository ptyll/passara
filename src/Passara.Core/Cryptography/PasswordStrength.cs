namespace Passara.Core.Cryptography;

/// <summary>
/// Defines the strength levels for passwords based on entropy calculation.
/// </summary>
public enum PasswordStrength
{
    /// <summary>
    /// Very weak password - less than 40 bits of entropy.
    /// </summary>
    VeryWeak = 1,

    /// <summary>
    /// Weak password - 40 to 60 bits of entropy.
    /// </summary>
    Weak = 2,

    /// <summary>
    /// Fair password - 60 to 80 bits of entropy.
    /// </summary>
    Fair = 3,

    /// <summary>
    /// Strong password - 80 to 120 bits of entropy.
    /// </summary>
    Strong = 4,

    /// <summary>
    /// Very strong password - more than 120 bits of entropy.
    /// </summary>
    VeryStrong = 5
}
