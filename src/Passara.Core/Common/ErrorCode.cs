namespace Passara.Core.Common;

/// <summary>
/// Defines all possible error codes used throughout the application.
/// </summary>
public enum ErrorCode
{
    None = 0,

    // Authentication errors (1000-1999)
    InvalidMasterPassword = 1001,
    VaultLocked = 1002,
    BiometricFailed = 1003,
    TooManyFailedAttempts = 1004,
    AccountLocked = 1005,

    // Vault errors (2000-2999)
    VaultNotFound = 2001,
    VaultCorrupted = 2002,
    VaultAlreadyExists = 2003,
    VaultVersionTooNew = 2004,
    VaultTooLarge = 2005,
    EntryNotFound = 2006,
    EntryAlreadyExists = 2007,

    // Cryptography errors (3000-3999)
    EncryptionFailed = 3001,
    DecryptionFailed = 3002,
    InvalidKey = 3003,
    WeakPassword = 3004,
    InvalidSalt = 3005,
    InvalidNonce = 3006,
    AlgorithmNotSupported = 3007,

    // Sync errors (4000-4999)
    SyncFailed = 4001,
    NetworkError = 4002,
    AuthenticationExpired = 4003,
    ConflictResolutionFailed = 4004,
    QuotaExceeded = 4005,
    RemoteVaultNotFound = 4006,

    // Storage errors (5000-5999)
    DiskFull = 5001,
    PermissionDenied = 5002,
    PathTooLong = 5003,
    FileNotFound = 5004,
    AccessDenied = 5005,

    // Import/Export errors (6000-6999)
    ImportFailed = 6001,
    ExportFailed = 6002,
    UnsupportedFormat = 6003,
    CorruptedImportData = 6004,

    // General errors (9000-9999)
    InvalidArgument = 9001,
    NotImplemented = 9002,
    OperationCancelled = 9003,
    InvalidOperation = 9004,
    Timeout = 9005,
    UnknownError = 9999
}
