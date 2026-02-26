namespace Passara.Core.Vault;

/// <summary>
/// Defines the version of the vault file format.
/// </summary>
public enum VaultFormatVersion
{
    /// <summary>
    /// Legacy format - not supported for writing.
    /// </summary>
    V1 = 1,

    /// <summary>
    /// Current format version.
    /// </summary>
    V2 = 2,

    /// <summary>
    /// Future format version - reserved.
    /// </summary>
    V3 = 3
}
