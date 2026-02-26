# Sprint 1: Core Cryptography Engine

**Cíl:** Implementovat bankovní standard kryptografie - KDF, symetrické šifrování, secure random, key management.

**Délka:** 5-7 dní  
**TDD přístup:** ✅ ANO - každá funkce nejprve testy, pak implementace  
**Dependencies:** Sprint 0

---

## 🔐 Security Requirements

- Použít **libsodium** (Sodium.Core NuGet) - žádná vlastní kryptografie!
- Všechny klíče v `SecureMemory` (zeroizace po použití)
- Constant-time comparison pro hesla
- Side-channel resistant implementace

---

## ✅ Tasky (TDD: Test → Implement → Refactor)

### 1. Secure Memory Management
**Testy jako první:**
- [x] `SecureBufferTests` - alokace, zeroizace, dispose pattern
- [x] `SensitiveDataTests` - `ISensitiveData` interface compliance
- [x] `MemoryProtectionTests` - ověření že paměť je skutečně vynulována

**Implementace:**
- [x] `ISecureBuffer` interface
- [x] `SecureBuffer` class (wrapper nad pinned managed memory)
- [x] `SensitiveData{T}` record pro typed secure data
- [x] `CryptoConstants.SecureMemoryAlignment`

### 2. Key Derivation Functions (KDF)
**Testy:**
- [x] `Argon2IdTests.DeriveKey_CorrectInput_Returns32Bytes`
- [x] `Argon2IdTests.DeriveKey_SameInputSameSalt_ReturnsSameKey`
- [x] `Argon2IdTests.DeriveKey_DifferentSalts_ReturnDifferentKeys`
- [x] `Argon2IdTests.DeriveKey_InvalidParameters_Throws`
- [x] `Pbkdf2Tests` - legacy support pro import

**Implementace:**
- [x] `IKdfProvider` interface
- [x] `Argon2IdProvider` s parametry:
  - `OpsLimit = KdfParameters.Argon2Operations`
  - `MemLimit = KdfParameters.Argon2MemoryBytes`
  - `Threads = KdfParameters.Argon2Parallelism`
- [x] `KdfOptions` record (immutable)
- [x] `KdfAlgorithmSelector` factory pattern

**Enums:**
```csharp
public enum KdfAlgorithmType
{
    Argon2Id = 1,
    Pbkdf2Sha256 = 2,  // Legacy only
    Pbkdf2Sha512 = 3   // Legacy only
}

public static class KdfParameters
{
    public const int Argon2Operations = 3;
    public const int Argon2MemoryBytes = 67108864; // 64 MB
    public const int Argon2Parallelism = 4;
    public const int KeyLength = 32;
    public const int SaltLength = 16;
}
```

### 3. Symmetric Encryption
**Testy:**
- [x] `AesGcmEncryptionTests.Encrypt_Decrypt_ReturnsOriginal`
- [x] `AesGcmEncryptionTests.Decrypt_TamperedData_Throws`
- [x] `AesGcmEncryptionTests.Decrypt_WrongKey_Throws`
- [x] `AesGcmEncryptionTests.Encrypt_EmptyPlaintext_Works`
- [x] `ChaCha20EncryptionTests` - alternativní algoritmus

**Implementace:**
- [x] `ISymmetricCipher` interface
- [x] `Aes256GcmCipher` (preferred)
- [x] `ChaCha20Poly1305Cipher` (mobile/low-power)
- [x] `EncryptedBlob` record s header + ciphertext + tag
- [x] `CipherAlgorithmSelector` factory

**Enums:**
```csharp
public enum CipherAlgorithm
{
    Aes256Gcm = 1,
    ChaCha20Poly1305 = 2
}

public static class EncryptionConstants
{
    public const int AesKeyLength = 32;
    public const int AesNonceLength = 12;
    public const int AesTagLength = 16;
    public const int ChaChaKeyLength = 32;
    public const int ChaChaNonceLength = 8; // Original ChaCha20 uses 8-byte nonce
}
```

### 4. Secure Random Generation
**Testy:**
- [x] `SecureRandomTests.GenerateBytes_ReturnsDifferentValues`
- [x] `SecureRandomTests.GeneratePassword_LengthCorrect`
- [x] `SecureRandomTests.GeneratePassword_ContainsRequiredCharsets`

**Implementace:**
- [x] `ISecureRandom` interface
- [x] `LibsodiumRandom` (using `randombytes_buf`)
- [x] `PasswordGenerator` s charakterovými sadami:
  - `Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"`
  - `Lowercase = "abcdefghijklmnopqrstuvwxyz"`
  - `Digits = "0123456789"`
  - `Special = "!@#$%^&*()_+-=[]{}|;:,.<>?~`'\"/\\"`

**Enums:**
```csharp
[Flags]
public enum PasswordCharacterSet
{
    None = 0,
    Uppercase = 1,
    Lowercase = 2,
    Digits = 4,
    Special = 8,
    All = Uppercase | Lowercase | Digits | Special
}

public static class PasswordGenerationDefaults
{
    public const int MinLength = 8;
    public const int DefaultLength = 16;
    public const int MaxLength = 128;
}
```

### 5. Master Key Management
**Testy:**
- [x] `MasterKeyTests.DeriveFromPassword_CorrectPassword_ReturnsKey`
- [x] `MasterKeyTests.DeriveFromPassword_WrongPassword_ReturnsNull`
- [x] `MasterKeyTests.Clear_ZeroizesMemory`

**Implementace:**
- [x] `MasterKey` class (wrapper nad byte[] s IDisposable)
- [x] `MasterKeyDerivationService`:
  - Input: `byte[] password`, `byte[] salt`
  - Output: `Result<MasterKey>`
- [x] `KeyDerivationStrength` - progress reporting pro UI

### 6. Cryptographic Validation
**Testy:**
- [x] `CryptoValidatorTests.ValidateKeyLength_Valid_ReturnsTrue`
- [x] `CryptoValidatorTests.ValidateKeyLength_Invalid_ReturnsFalse`

**Implementace:**
- [x] `ICryptoValidator` interface
- [x] `CryptoValidator` - kontrola délek, formátů

---

## 🎨 UI/UX Specifikace

### Progress Indicators pro KDF
Protože Argon2 může trvat 500ms-2s na mobilních zařízeních:

```
┌─────────────────────────────────────┐
│ 🔐 Odemykání trezoru                │
│                                     │
│ [████████████████░░░░] 75%          │
│ Zabezpečení klíče...                │
│                                     │
│ (Tato operace je záměrně pomalá     │
│  pro vaši bezpečnost)               │
└─────────────────────────────────────┘
```

**UX principy:**
- Vždy vysvětli PROČ to trvá (security education)
- Progress bar s indeterministickým koncem (pokud neznáme přesný čas)
- Cancel button s potvrzením ("Opravdu chcete přerušit?")

### Password Generator Preview
```
┌──────────────────────────────────────────┐
│ 🔧 Generátor hesla                        │
│ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ │
│                                          │
│ Délka: [━━━●━━━━] 16 znaků              │
│                                          │
│ ☑️ Velká písmena (A-Z)                   │
│ ☑️ Malá písmena (a-z)                    │
│ ☑️ Číslice (0-9)                         │
│ ☐ Speciální znaky (!@#$)                 │
│                                          │
│ ┌──────────────────────────────────┐    │
│ │ Tr0ub4dor&3!  [🔄] [📋]          │    │
│ │ Entropie: 85 bitů (Velmi silné)  │    │
│ └──────────────────────────────────┘    │
└──────────────────────────────────────────┘
```

---

## 📝 Příklad použití (API design)

```csharp
// KDF
var kdf = new Argon2IdProvider();
var result = await kdf.DeriveKeyAsync(
    password: masterPassword,
    salt: salt,
    options: KdfOptions.Moderate,
    progress: progressReporter,
    cancellationToken: ct);

if (result.IsFailure)
    return Result.Failure(result.ErrorCode);

// Encryption
var cipher = new Aes256GcmCipher();
var encrypted = cipher.Encrypt(plaintext: data, key: key, associatedData: header);

// Password generation
var generator = new PasswordGenerator(LibsodiumRandom.Instance);
var password = generator.Generate(
    length: 20,
    characterSets: PasswordCharacterSet.All,
    requireAllTypes: true);
```

---

## 📋 Definition of Done

- [x] 100% code coverage pro kryptografické funkce
- [x] Security review: žádné hardcoded keys, žádné predictable random
- [x] Performance test: Argon2 < 1s na cílovém hardwaru
- [x] Memory dump test: žádné plaintext hesla v paměti po dispose
- [x] Všechny testy zelené (178 testů)
