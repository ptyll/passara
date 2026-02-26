using Sodium;

namespace Passara.Core.Cryptography;

/// <summary>
/// Provides cryptographically secure random number generation using libsodium.
/// </summary>
public sealed class LibsodiumRandom : ISecureRandom
{
    private static readonly Lazy<LibsodiumRandom> _instance = new(() => new LibsodiumRandom());

    /// <summary>
    /// Gets the singleton instance of <see cref="LibsodiumRandom"/>.
    /// </summary>
    public static ISecureRandom Instance => _instance.Value;

    /// <inheritdoc />
    public byte[] GenerateBytes(int count)
    {
        if (count < 0)
        {
            throw new ArgumentException("Count cannot be negative.", nameof(count));
        }

        if (count == 0)
        {
            return Array.Empty<byte>();
        }

        return SodiumCore.GetRandomBytes(count);
    }

    /// <inheritdoc />
    public uint GenerateUInt32()
    {
        var bytes = GenerateBytes(4);
        return BitConverter.ToUInt32(bytes, 0);
    }

    /// <inheritdoc />
    public uint GenerateUniform(uint max)
    {
        if (max == 0)
        {
            throw new ArgumentException("Max must be greater than 0.", nameof(max));
        }

        // Use rejection sampling for uniform distribution
        uint result;
        do
        {
            result = GenerateUInt32();
        } while (result >= uint.MaxValue - (uint.MaxValue % max));

        return result % max;
    }

    /// <inheritdoc />
    public void Shuffle<T>(T[] array)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (array.Length <= 1)
        {
            return;
        }

        // Fisher-Yates shuffle
        for (int i = array.Length - 1; i > 0; i--)
        {
            var j = (int)(GenerateUniform((uint)(i + 1)));
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
