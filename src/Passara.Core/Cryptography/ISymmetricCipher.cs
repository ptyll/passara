using Passara.Core.Common;

namespace Passara.Core.Cryptography;

/// <summary>
/// Provides symmetric encryption operations.
/// </summary>
public interface ISymmetricCipher
{
    /// <summary>
    /// Gets the algorithm used by this cipher.
    /// </summary>
    CipherAlgorithm Algorithm { get; }

    /// <summary>
    /// Encrypts the specified plaintext.
    /// </summary>
    /// <param name="plaintext">The data to encrypt.</param>
    /// <param name="key">The encryption key.</param>
    /// <param name="associatedData">Optional associated data for authentication.</param>
    /// <returns>A result containing the encrypted blob or an error.</returns>
    Result<EncryptedBlob> Encrypt(byte[] plaintext, byte[] key, byte[]? associatedData = null);

    /// <summary>
    /// Decrypts the specified encrypted blob.
    /// </summary>
    /// <param name="encryptedBlob">The encrypted data.</param>
    /// <param name="key">The decryption key.</param>
    /// <param name="associatedData">Optional associated data for authentication.</param>
    /// <returns>A result containing the decrypted plaintext or an error.</returns>
    Result<byte[]> Decrypt(EncryptedBlob encryptedBlob, byte[] key, byte[]? associatedData = null);
}

/// <summary>
/// Represents an encrypted data blob containing nonce, ciphertext, and authentication tag.
/// </summary>
public sealed record EncryptedBlob
{
    /// <summary>
    /// Gets the nonce (initialization vector) used for encryption.
    /// </summary>
    public byte[] Nonce { get; }

    /// <summary>
    /// Gets the encrypted ciphertext.
    /// </summary>
    public byte[] Ciphertext { get; }

    /// <summary>
    /// Gets the authentication tag.
    /// </summary>
    public byte[] Tag { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptedBlob"/> class.
    /// </summary>
    /// <param name="nonce">The nonce.</param>
    /// <param name="ciphertext">The ciphertext.</param>
    /// <param name="tag">The authentication tag.</param>
    public EncryptedBlob(byte[] nonce, byte[] ciphertext, byte[] tag)
    {
        Nonce = nonce ?? throw new ArgumentNullException(nameof(nonce));
        Ciphertext = ciphertext ?? throw new ArgumentNullException(nameof(ciphertext));
        Tag = tag ?? throw new ArgumentNullException(nameof(tag));
    }

    /// <summary>
    /// Combines the nonce, ciphertext, and tag into a single byte array.
    /// </summary>
    /// <returns>The combined byte array.</returns>
    public byte[] ToByteArray()
    {
        var result = new byte[Nonce.Length + Ciphertext.Length + Tag.Length];
        Buffer.BlockCopy(Nonce, 0, result, 0, Nonce.Length);
        Buffer.BlockCopy(Ciphertext, 0, result, Nonce.Length, Ciphertext.Length);
        Buffer.BlockCopy(Tag, 0, result, Nonce.Length + Ciphertext.Length, Tag.Length);
        return result;
    }

    /// <summary>
    /// Creates an <see cref="EncryptedBlob"/> from a combined byte array.
    /// </summary>
    /// <param name="data">The combined byte array.</param>
    /// <param name="nonceLength">The length of the nonce.</param>
    /// <param name="tagLength">The length of the tag.</param>
    /// <returns>A new <see cref="EncryptedBlob"/>.</returns>
    public static EncryptedBlob FromByteArray(byte[] data, int nonceLength, int tagLength)
    {
        if (data.Length < nonceLength + tagLength)
        {
            throw new ArgumentException("Data is too short to contain nonce and tag.", nameof(data));
        }

        var nonce = new byte[nonceLength];
        var tag = new byte[tagLength];
        var ciphertextLength = data.Length - nonceLength - tagLength;
        var ciphertext = new byte[ciphertextLength];

        Buffer.BlockCopy(data, 0, nonce, 0, nonceLength);
        Buffer.BlockCopy(data, nonceLength, ciphertext, 0, ciphertextLength);
        Buffer.BlockCopy(data, nonceLength + ciphertextLength, tag, 0, tagLength);

        return new EncryptedBlob(nonce, ciphertext, tag);
    }
}
