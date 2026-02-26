namespace Passara.Core.Cryptography;

/// <summary>
/// Provides cryptographically secure random number generation.
/// </summary>
public interface ISecureRandom
{
    /// <summary>
    /// Generates a specified number of random bytes.
    /// </summary>
    /// <param name="count">The number of bytes to generate.</param>
    /// <returns>An array of random bytes.</returns>
    byte[] GenerateBytes(int count);

    /// <summary>
    /// Generates a random 32-bit unsigned integer.
    /// </summary>
    /// <returns>A random 32-bit unsigned integer.</returns>
    uint GenerateUInt32();

    /// <summary>
    /// Generates a random 32-bit unsigned integer less than the specified maximum.
    /// </summary>
    /// <param name="max">The exclusive upper bound.</param>
    /// <returns>A random 32-bit unsigned integer less than max.</returns>
    uint GenerateUniform(uint max);

    /// <summary>
    /// Shuffles the elements of the specified array randomly.
    /// </summary>
    /// <typeparam name="T">The type of array elements.</typeparam>
    /// <param name="array">The array to shuffle.</param>
    void Shuffle<T>(T[] array);
}
