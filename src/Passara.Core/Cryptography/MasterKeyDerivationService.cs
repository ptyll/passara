using Passara.Core.Common;

namespace Passara.Core.Cryptography;

/// <summary>
/// Service for deriving master keys from passwords.
/// </summary>
public sealed class MasterKeyDerivationService
{
    private readonly IKdfProvider _kdfProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="MasterKeyDerivationService"/> class.
    /// </summary>
    public MasterKeyDerivationService()
    {
        _kdfProvider = new Argon2IdProvider();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MasterKeyDerivationService"/> class.
    /// </summary>
    /// <param name="kdfProvider">The KDF provider to use.</param>
    public MasterKeyDerivationService(IKdfProvider kdfProvider)
    {
        _kdfProvider = kdfProvider ?? throw new ArgumentNullException(nameof(kdfProvider));
    }

    /// <summary>
    /// Derives a master key from a password and salt.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="salt">The salt.</param>
    /// <param name="strength">The derivation strength level.</param>
    /// <param name="progress">Optional progress reporter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the master key or an error.</returns>
    public async Task<Result<MasterKey>> DeriveFromPasswordAsync(
        byte[] password,
        byte[] salt,
        KeyDerivationStrength strength,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (password == null)
        {
            return Result<MasterKey>.Failure(ErrorCode.InvalidArgument, "Password cannot be null.");
        }

        if (salt == null)
        {
            return Result<MasterKey>.Failure(ErrorCode.InvalidArgument, "Salt cannot be null.");
        }

        if (salt.Length != KdfParameters.SaltLength)
        {
            return Result<MasterKey>.Failure(ErrorCode.InvalidArgument, $"Salt must be exactly {KdfParameters.SaltLength} bytes.");
        }

        // Get KDF options based on strength
        var options = strength switch
        {
            KeyDerivationStrength.Interactive => KdfOptions.Interactive,
            KeyDerivationStrength.Moderate => KdfOptions.Moderate,
            KeyDerivationStrength.Sensitive => KdfOptions.Sensitive,
            _ => KdfOptions.Moderate
        };

        // Derive the key
        var deriveResult = await _kdfProvider.DeriveKeyAsync(password, salt, options, progress, cancellationToken);

        if (deriveResult.IsFailure)
        {
            return Result<MasterKey>.Failure(
                deriveResult.ErrorCode,
                deriveResult.ErrorMessage ?? "Key derivation failed.");
        }

        // Wrap the key in a MasterKey
        var masterKey = new MasterKey(deriveResult.Value!);
        return Result<MasterKey>.Success(masterKey);
    }
}
