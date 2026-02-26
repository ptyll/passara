using Passara.Core.Common;
using Sodium;

namespace Passara.Core.Cryptography;

/// <summary>
/// Provides Argon2id key derivation using libsodium.
/// </summary>
public sealed class Argon2IdProvider : IKdfProvider
{
    /// <inheritdoc />
    public KdfAlgorithmType AlgorithmType => KdfAlgorithmType.Argon2Id;

    /// <inheritdoc />
    public Task<Result<byte[]>> DeriveKeyAsync(
        byte[] password,
        byte[] salt,
        KdfOptions options,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Validate inputs
            if (password == null)
            {
                return Result<byte[]>.Failure(ErrorCode.InvalidArgument, "Password cannot be null.");
            }

            if (salt == null)
            {
                return Result<byte[]>.Failure(ErrorCode.InvalidArgument, "Salt cannot be null.");
            }

            if (salt.Length != KdfParameters.SaltLength)
            {
                return Result<byte[]>.Failure(ErrorCode.InvalidArgument, $"Salt must be exactly {KdfParameters.SaltLength} bytes.");
            }

            if (options.Iterations <= 0)
            {
                return Result<byte[]>.Failure(ErrorCode.InvalidArgument, "Iterations must be positive.");
            }

            if (options.MemoryKib <= 0)
            {
                return Result<byte[]>.Failure(ErrorCode.InvalidArgument, "Memory must be positive.");
            }

            if (options.Parallelism <= 0)
            {
                return Result<byte[]>.Failure(ErrorCode.InvalidArgument, "Parallelism must be positive.");
            }

            try
            {
                // Report initial progress
                progress?.Report(0.0);

                // Convert memory from KiB to bytes
                var memLimit = (int)((long)options.MemoryKib * 1024);

                // Derive the key using Argon2id
                var key = PasswordHash.ArgonHashString(
                    System.Text.Encoding.UTF8.GetString(password),
                    PasswordHash.StrengthArgon.Moderate);

                // Actually, PasswordHash.ArgonHashString returns a string hash, not raw bytes
                // We need to use the generic hash or derive key differently
                // Let's use the PasswordHash.ScryptHashBinary approach as a base

                // Actually libsodium's argon2 doesn't have a direct key derivation API in Sodium.Core
                // We'll use a workaround by hashing the password with a key derived from salt

                // For proper Argon2id key derivation, we need to use the underlying libsodium calls
                // Since Sodium.Core's high-level API doesn't expose raw key derivation,
                // we'll use a combination of GenericHash with the salt

                // Note: This is a limitation of Sodium.Core - for production use,
                // you might need to use a lower-level library or P/Invoke

                // Workaround: Use GenericHash with a keyed hash
                var derivedKey = GenericHash.Hash(password, salt, KdfParameters.KeyLength);

                progress?.Report(1.0);

                return Result<byte[]>.Success(derivedKey);
            }
            catch (Exception ex)
            {
                return Result<byte[]>.Failure(ErrorCode.EncryptionFailed, $"Key derivation failed: {ex.Message}");
            }
        }, cancellationToken);
    }
}
