using System.Text;

namespace Passara.Core.Cryptography;

/// <summary>
/// Generates secure random passwords with configurable character sets.
/// </summary>
public sealed class PasswordGenerator
{
    /// <summary>
    /// Uppercase letters character set.
    /// </summary>
    public const string UppercaseCharset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// Lowercase letters character set.
    /// </summary>
    public const string LowercaseCharset = "abcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// Digits character set.
    /// </summary>
    public const string DigitsCharset = "0123456789";

    /// <summary>
    /// Special characters character set.
    /// </summary>
    public const string SpecialCharset = "!@#$%^&*()_+-=[]{}|;:,.<>?~`'\"/\\";

    private readonly ISecureRandom _random;

    /// <summary>
    /// Initializes a new instance of the <see cref="PasswordGenerator"/> class.
    /// </summary>
    /// <param name="random">The secure random number generator to use.</param>
    public PasswordGenerator(ISecureRandom random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    /// <summary>
    /// Generates a random password with the specified parameters.
    /// </summary>
    /// <param name="length">The length of the password.</param>
    /// <param name="characterSets">The character sets to use.</param>
    /// <param name="requireAllTypes">Whether to require at least one character from each selected set.</param>
    /// <returns>The generated password.</returns>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
    public string Generate(int length, PasswordCharacterSet characterSets, bool requireAllTypes = false)
    {
        // Validate length
        if (length < PasswordGenerationDefaults.MinLength)
        {
            throw new ArgumentException($"Password length must be at least {PasswordGenerationDefaults.MinLength}.", nameof(length));
        }

        if (length > PasswordGenerationDefaults.MaxLength)
        {
            throw new ArgumentException($"Password length cannot exceed {PasswordGenerationDefaults.MaxLength}.", nameof(length));
        }

        // Validate character sets
        if (characterSets == PasswordCharacterSet.None)
        {
            throw new ArgumentException("At least one character set must be selected.", nameof(characterSets));
        }

        // Build the character pool
        var pool = new StringBuilder();
        var requiredChars = new List<char>();

        if (characterSets.HasFlag(PasswordCharacterSet.Uppercase))
        {
            pool.Append(UppercaseCharset);
            if (requireAllTypes)
            {
                requiredChars.Add(UppercaseCharset[(int)_random.GenerateUniform((uint)UppercaseCharset.Length)]);
            }
        }

        if (characterSets.HasFlag(PasswordCharacterSet.Lowercase))
        {
            pool.Append(LowercaseCharset);
            if (requireAllTypes)
            {
                requiredChars.Add(LowercaseCharset[(int)_random.GenerateUniform((uint)LowercaseCharset.Length)]);
            }
        }

        if (characterSets.HasFlag(PasswordCharacterSet.Digits))
        {
            pool.Append(DigitsCharset);
            if (requireAllTypes)
            {
                requiredChars.Add(DigitsCharset[(int)_random.GenerateUniform((uint)DigitsCharset.Length)]);
            }
        }

        if (characterSets.HasFlag(PasswordCharacterSet.Special))
        {
            pool.Append(SpecialCharset);
            if (requireAllTypes)
            {
                requiredChars.Add(SpecialCharset[(int)_random.GenerateUniform((uint)SpecialCharset.Length)]);
            }
        }

        var poolStr = pool.ToString();
        if (poolStr.Length == 0)
        {
            throw new ArgumentException("Character pool is empty.", nameof(characterSets));
        }

        // Check if we can fit all required characters
        if (requireAllTypes && requiredChars.Count > length)
        {
            throw new ArgumentException($"Password length ({length}) is too short to include all required character types ({requiredChars.Count}).");
        }

        // Generate the password
        var passwordChars = new char[length];

        // First, place the required characters
        for (int i = 0; i < requiredChars.Count; i++)
        {
            passwordChars[i] = requiredChars[i];
        }

        // Fill the rest with random characters from the pool
        for (int i = requiredChars.Count; i < length; i++)
        {
            passwordChars[i] = poolStr[(int)_random.GenerateUniform((uint)poolStr.Length)];
        }

        // Shuffle the password to randomize the position of required characters
        _random.Shuffle(passwordChars);

        return new string(passwordChars);
    }

    /// <summary>
    /// Calculates the strength of a password based on its entropy.
    /// </summary>
    /// <param name="length">The length of the password.</param>
    /// <param name="characterSets">The character sets used.</param>
    /// <returns>The estimated password strength.</returns>
    public static PasswordStrength CalculateStrength(int length, PasswordCharacterSet characterSets)
    {
        // Calculate pool size
        int poolSize = 0;
        if (characterSets.HasFlag(PasswordCharacterSet.Uppercase))
        {
            poolSize += UppercaseCharset.Length;
        }
        if (characterSets.HasFlag(PasswordCharacterSet.Lowercase))
        {
            poolSize += LowercaseCharset.Length;
        }
        if (characterSets.HasFlag(PasswordCharacterSet.Digits))
        {
            poolSize += DigitsCharset.Length;
        }
        if (characterSets.HasFlag(PasswordCharacterSet.Special))
        {
            poolSize += SpecialCharset.Length;
        }

        if (poolSize == 0 || length == 0)
        {
            return PasswordStrength.VeryWeak;
        }

        // Calculate entropy: log2(poolSize^length) = length * log2(poolSize)
        double entropy = length * Math.Log(poolSize) / Math.Log(2);

        return entropy switch
        {
            < 40 => PasswordStrength.VeryWeak,
            < 60 => PasswordStrength.Weak,
            < 80 => PasswordStrength.Fair,
            < 120 => PasswordStrength.Strong,
            _ => PasswordStrength.VeryStrong
        };
    }
}
