# Enums and Constants Reference

Kompletní seznam všech enumů a konstant používaných v aplikaci.  
**Pravidlo:** Žádná magická čísla v kódu - vše musí být definováno zde nebo v příslušných konstantních třídách.

---

## 🎨 UI / Theme

```csharp
public enum AppTheme
{
    System = 0,
    Light = 1,
    Dark = 2
}

public enum AccentColor
{
    Blue = 1,
    Purple = 2,
    Green = 3,
    Orange = 4,
    Red = 5,
    Pink = 6
}

public enum WindowState
{
    Normal = 0,
    Minimized = 1,
    Maximized = 2,
    FullScreen = 3
}
```

---

## 🔐 Security & Cryptography

```csharp
public enum KdfAlgorithmType
{
    Argon2Id = 1,
    Pbkdf2Sha256 = 2,
    Pbkdf2Sha512 = 3
}

public enum CipherAlgorithm
{
    Aes256Gcm = 1,
    ChaCha20Poly1305 = 2
}

public enum KeyDerivationStrength
{
    Interactive = 1,    // Rychlé (mobilní)
    Moderate = 2,       // Vyvážené (default)
    Sensitive = 3       // Paranoidní (pomalé)
}

public enum PasswordStrength
{
    VeryWeak = 1,    // < 40 bits
    Weak = 2,        // 40-60 bits
    Fair = 3,        // 60-80 bits
    Strong = 4,      // 80-120 bits
    VeryStrong = 5   // > 120 bits
}

public enum BiometricType
{
    None = 0,
    Fingerprint = 1,
    Face = 2,
    Iris = 3,
    Voice = 4
}

public enum BiometricStatus
{
    Available = 1,
    NotAvailable = 2,
    NotEnrolled = 3,
    PermissionDenied = 4,
    HardwareUnavailable = 5
}

public enum BiometricResult
{
    Success = 1,
    Cancelled = 2,
    Failed = 3,
    TooManyAttempts = 4,
    Lockout = 5
}

public static class KdfParameters
{
    public const int Argon2InteractiveMemoryKib = 65536;      // 64 MB
    public const int Argon2InteractiveIterations = 2;
    public const int Argon2InteractiveParallelism = 1;
    
    public const int Argon2ModerateMemoryKib = 262144;        // 256 MB
    public const int Argon2ModerateIterations = 3;
    public const int Argon2ModerateParallelism = 4;
    
    public const int Argon2SensitiveMemoryKib = 1048576;      // 1 GB
    public const int Argon2SensitiveIterations = 4;
    public const int Argon2SensitiveParallelism = 4;
    
    public const int SaltLength = 16;
    public const int KeyLength = 32;
}

public static class EncryptionConstants
{
    public const int AesKeyLength = 32;
    public const int AesNonceLength = 12;
    public const int AesTagLength = 16;
    
    public const int ChaChaKeyLength = 32;
    public const int ChaChaNonceLength = 12;
    
    public const int MasterKeyLength = 32;
}

public static class SecurityLimits
{
    public const int MinMasterPasswordLength = 8;
    public const int RecommendedMasterPasswordLength = 12;
    public const int MaxMasterPasswordLength = 128;
    
    public const int MaxFailedUnlockAttempts = 5;
    public const int MaxFailedAttemptsBeforeLockout = 5;
    
    public static readonly TimeSpan MinKdfDuration = TimeSpan.FromMilliseconds(500);
    public static readonly TimeSpan MaxKdfDuration = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan ClipboardClearDelay = TimeSpan.FromSeconds(10);
    public static readonly TimeSpan AutoLockDefaultDelay = TimeSpan.FromMinutes(10);
}
```

---

## 🗃️ Vault & Data

```csharp
public enum VaultFormatVersion
{
    V1 = 1,  // Legacy
    V2 = 2,  // Current
    V3 = 3   // Future
}

public enum EntryType
{
    Password = 1,
    SecureNote = 2,
    CreditCard = 3,
    Identity = 4,
    BankAccount = 5,
    Passport = 6,
    DriversLicense = 7,
    SoftwareLicense = 8,
    SshKey = 9,
    Database = 10,
    ApiCredential = 11,
    WifiPassword = 12
}

public enum FieldType
{
    Text = 1,
    Password = 2,
    Email = 3,
    Url = 4,
    Phone = 5,
    Date = 6,
    Number = 7,
    Boolean = 8,
    Multiline = 9,
    Totp = 10,
    Tags = 11,
    Color = 12
}

public enum CardType
{
    Visa = 1,
    Mastercard = 2,
    Amex = 3,
    Discover = 4,
    DinersClub = 5,
    Jcb = 6,
    UnionPay = 7,
    Other = 8
}

public enum VaultRepositoryError
{
    None = 0,
    FileNotFound = 1,
    AccessDenied = 2,
    CorruptedVault = 3,
    WrongPassword = 4,
    VersionTooNew = 5,
    DiskFull = 6,
    VaultTooLarge = 7,
    AlreadyExists = 8
}

public static class VaultLimits
{
    public const int MaxEntries = 10000;
    public const int MaxAttachmentsPerEntry = 10;
    public const long MaxAttachmentSize = 50 * 1024 * 1024;      // 50 MB
    public const long MaxTotalAttachmentSize = 500 * 1024 * 1024; // 500 MB
    public const int MaxCustomFieldsPerEntry = 20;
    public const int MaxFieldLength = 10000;                     // 10KB text
    public const int MaxHistoryEntries = 100;
    public const int MaxFolders = 100;
}

public static class PasswordGenerationPresets
{
    public const int LengthPin = 6;
    public const int LengthShort = 8;
    public const int LengthDefault = 16;
    public const int LengthLong = 32;
    public const int LengthMaximum = 128;
    
    public const int PassphraseWordsMin = 3;
    public const int PassphraseWordsDefault = 4;
    public const int PassphraseWordsMax = 10;
}
```

---

## 🔢 TOTP

```csharp
public enum TotpAlgorithm
{
    Sha1 = 1,
    Sha256 = 2,
    Sha512 = 3
}

public static class TotpConstants
{
    public const int DefaultPeriodSeconds = 30;
    public const int DefaultDigits = 6;
    public const int SteamDigits = 5;
    public const int MaxDigits = 8;
    public const int MaxDriftSteps = 2;  // ±1 period tolerance
    public const int WarningThresholdSeconds = 5;
}
```

---

## ☁️ Sync

```csharp
public enum CloudProviderType
{
    LocalFolder = 1,
    OneDrive = 2,
    GoogleDrive = 3,
    Dropbox = 4,
    ICloud = 5,
    WebDav = 6,
    S3Compatible = 7,
    Nextcloud = 8
}

public enum SyncStatus
{
    Idle = 0,
    Checking = 1,
    Downloading = 2,
    Uploading = 3,
    ResolvingConflicts = 4,
    Error = 5,
    Disabled = 6,
    Paused = 7
}

public enum SyncDirection
{
    UploadOnly = 1,
    DownloadOnly = 2,
    Bidirectional = 3
}

public enum SyncConflictResolution
{
    UseLocal = 1,
    UseRemote = 2,
    Merge = 3,
    AskUser = 4,
    KeepBoth = 5
}

public enum ConflictType
{
    BothModified = 1,
    LocalModifiedRemoteDeleted = 2,
    LocalDeletedRemoteModified = 3,
    AttachmentMismatch = 4,
    VersionMismatch = 5
}

public enum SyncError
{
    None = 0,
    NetworkError = 1,
    AuthenticationExpired = 2,
    QuotaExceeded = 3,
    FileLocked = 4,
    InvalidRemoteData = 5,
    Timeout = 6
}

public static class SyncConstants
{
    public static readonly TimeSpan AutoSyncInterval = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan MinSyncInterval = TimeSpan.FromSeconds(30);
    public static readonly TimeSpan SyncTimeout = TimeSpan.FromMinutes(2);
    public const int MaxRetries = 3;
    public const int RetryBaseDelayMs = 1000;
}
```

---

## 🌐 Browser Extension

```csharp
public enum HostAction
{
    GetCredentials = 1,
    SaveCredentials = 2,
    GeneratePassword = 3,
    CheckStatus = 4,
    FillForm = 5,
    GetTotp = 6,
    LockVault = 7,
    UnlockVault = 8
}

public enum FormFieldType
{
    Unknown = 0,
    Username = 1,
    Email = 2,
    Password = 3,
    PasswordConfirm = 4,
    CurrentPassword = 5,
    NewPassword = 6,
    Totp = 7,
    Submit = 8,
    Search = 9,
    CreditCardNumber = 10,
    CreditCardExpiry = 11,
    CreditCardCvv = 12
}

public enum FormType
{
    Unknown = 0,
    Login = 1,
    Registration = 2,
    PasswordChange = 3,
    PasswordReset = 4,
    Payment = 5,
    Identity = 6,
    Search = 7
}

public enum AutoTypeSequenceType
{
    UsernameOnly = 1,
    PasswordOnly = 2,
    UsernameThenTabThenPassword = 3,
    UsernameThenTabThenPasswordThenEnter = 4,
    Custom = 5
}

public static class AutoTypeConstants
{
    public const int DefaultDelayMs = 10;
    public const int FieldDelayMs = 50;
    public const int FormDelayMs = 100;
    public const int MaxSequenceLength = 1000;
}
```

---

## 🖥️ Desktop Specific

```csharp
public enum HotkeyAction
{
    QuickSearch = 1,
    AutoType = 2,
    LockVault = 3,
    CopyPassword = 4,
    CopyUsername = 5,
    CopyTotp = 6,
    ShowWindow = 7
}

public enum ModifierKey
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Windows = 8
}

public enum VaultLockTrigger
{
    Manual = 1,
    SystemLock = 2,
    IdleTimeout = 3,
    ClipboardClear = 4,
    AppMinimize = 5,
    AppExit = 6,
    RemoteRequest = 7
}

public enum IdleTimeoutDuration
{
    Never = 0,
    OneMinute = 1,
    FiveMinutes = 2,
    TenMinutes = 3,
    FifteenMinutes = 4,
    ThirtyMinutes = 5,
    OneHour = 6
}

public enum WindowCorner
{
    TopLeft = 1,
    TopRight = 2,
    BottomLeft = 3,
    BottomRight = 4
}

public static class DesktopConstants
{
    public const int QuickSearchMaxResults = 10;
    public const int QuickSearchMinQueryLength = 2;
    public static readonly TimeSpan QuickSearchDebounce = TimeSpan.FromMilliseconds(150);
    public static readonly TimeSpan ToastDisplayDuration = TimeSpan.FromSeconds(3);
    public static readonly TimeSpan ErrorToastDisplayDuration = TimeSpan.FromSeconds(8);
}
```

---

## 📱 Mobile Specific

```csharp
public enum MobileNavigationTab
{
    Vault = 1,
    Generator = 2,
    Settings = 3,
    Favorites = 4
}

public enum SwipeDirection
{
    Left = 1,
    Right = 2,
    Up = 3,
    Down = 4
}

public enum TouchState
{
    None = 0,
    Tap = 1,
    LongPress = 2,
    Swipe = 3,
    Pinch = 4
}

public static class MobileConstants
{
    public const int MinTouchTargetSizeDp = 48;
    public const int SwipeThresholdDp = 100;
    public const int LongPressDurationMs = 500;
    public const int AnimationDurationMs = 200;
    public const int MaxSearchResults = 50;
}
```

---

## 📝 Audit & Logging

```csharp
public enum AuditEventType
{
    VaultCreated = 1,
    VaultUnlocked = 2,
    VaultLocked = 3,
    VaultDeleted = 4,
    EntryCreated = 5,
    EntryModified = 6,
    EntryDeleted = 7,
    EntryViewed = 8,
    EntryCopied = 9,
    PasswordCopied = 10,
    UsernameCopied = 11,
    TotpCopied = 12,
    ExportPerformed = 13,
    ImportPerformed = 14,
    SyncStarted = 15,
    SyncCompleted = 16,
    SyncFailed = 17,
    SettingsChanged = 18,
    BackupCreated = 19,
    EmergencyAccessUsed = 20
}

public enum LogLevel
{
    Verbose = 0,
    Debug = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
    Fatal = 5
}

public static class AuditConstants
{
    public const int MaxLogEntries = 10000;
    public const int MaxLogAgeDays = 365;
    public static readonly TimeSpan LogFlushInterval = TimeSpan.FromMinutes(5);
}
```

---

## 🌍 Localization

```csharp
public enum SupportedLanguage
{
    English = 1,
    Czech = 2,
    German = 3,
    French = 4,
    Spanish = 5,
    Russian = 6,
    ChineseSimplified = 7,
    Japanese = 8
}

public enum DateFormat
{
    Iso8601 = 1,           // 2026-02-26
    UsShort = 2,           // 2/26/2026
    European = 3,          // 26.2.2026
    FullLocalized = 4      // 26. února 2026
}

public enum TimeFormat
{
    H24 = 1,               // 14:30
    H12 = 2                // 2:30 PM
}
```

---

## 📦 Import/Export

```csharp
public enum ImportFormat
{
    Json = 1,
    Csv = 2,
    EnpassJson = 3,
    BitwardenJson = 4,
    LastPassCsv = 5,
    ChromeCsv = 6,
    FirefoxCsv = 7,
    KeePassXml = 8,
    OnePassword = 9,
    Dashlane = 10
}

public enum ExportFormat
{
    EncryptedJson = 1,
    UnencryptedJson = 2,
    Csv = 3,
    Pdf = 4,
    Txt = 5
}

public enum ExportWarningLevel
{
    None = 0,
    Information = 1,
    Warning = 2,
    Critical = 3
}

public enum ImportResult
{
    Success = 1,
    PartialSuccess = 2,
    InvalidFormat = 3,
    CorruptedData = 4,
    UnsupportedVersion = 5,
    Cancelled = 6
}
```

---

## ⚡ Performance

```csharp
public static class PerformanceTargets
{
    public const int MaxUnlockTimeMs = 2000;
    public const int MaxSearchTimeMs = 100;
    public const int MaxSaveTimeMs = 200;
    public const int MaxSyncTimeMs = 3000;
    
    public const long MaxDesktopMemoryBytes = 200 * 1024 * 1024;    // 200 MB
    public const long MaxMobileMemoryBytes = 100 * 1024 * 1024;     // 100 MB
    
    public const int MaxConcurrentOperations = 5;
    public const int BackgroundTaskTimeoutMs = 30000;
}
```

---

## 🔌 Result & Error Handling

```csharp
public enum ErrorCode
{
    None = 0,
    
    // Authentication
    InvalidMasterPassword = 1001,
    VaultLocked = 1002,
    BiometricFailed = 1003,
    TooManyFailedAttempts = 1004,
    
    // Vault
    VaultNotFound = 2001,
    VaultCorrupted = 2002,
    VaultAlreadyExists = 2003,
    VaultVersionTooNew = 2004,
    
    // Crypto
    EncryptionFailed = 3001,
    DecryptionFailed = 3002,
    InvalidKey = 3003,
    WeakPassword = 3004,
    
    // Sync
    SyncFailed = 4001,
    NetworkError = 4002,
    AuthenticationExpired = 4003,
    ConflictResolutionFailed = 4004,
    
    // Storage
    DiskFull = 5001,
    PermissionDenied = 5002,
    PathTooLong = 5003,
    
    // General
    InvalidArgument = 9001,
    NotImplemented = 9002,
    OperationCancelled = 9003,
    UnknownError = 9999
}
```

---

## Použití v kódu

```csharp
// ✅ Správně:
if (passwordStrength == PasswordStrength.VeryWeak)
{
    return Result.Failure(ErrorCode.WeakPassword);
}

var delay = KdfParameters.Argon2ModerateIterations;

// ❌ Špatně:
if (strength == 1) { ... }  // Magic number!
if (delay == 3) { ... }     // Magic number!
```
