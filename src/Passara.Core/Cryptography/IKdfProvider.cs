using Passara.Core.Common;

namespace Passara.Core.Cryptography;

/// <summary>
/// Provides key derivation function (KDF) operations.
/// </summary>
public interface IKdfProvider
{
    /// <summary>
    /// Gets the algorithm type used by this provider.
    /// </summary>
    KdfAlgorithmType AlgorithmType { get; }

    /// <summary>
    /// Derives a key from a password and salt using the configured KDF algorithm.
    /// </summary>
    /// <param name="password">The password to derive the key from.</param>
    /// <param name="salt">The salt to use. Must be exactly <see cref="KdfParameters.SaltLength"/> bytes.</param>
    /// <param name="options">The KDF parameters to use.</param>
    /// <param name="progress">Optional progress reporter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the derived key or an error.</returns>
    Task<Result<byte[]>> DeriveKeyAsync(
        byte[] password,
        byte[] salt,
        KdfOptions options,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default);
}
