# Sprint 2: Data Layer & Vault Models

**Cíl:** Domain models, encrypted storage format, repository pattern, import/export capabilities.

**Délka:** 5-6 dní  
**TDD přístup:** ✅ ANO  
**Dependencies:** Sprint 1

---

## 🗃️ Domain Models

### Vault Hierarchy
```
Vault
├── Metadata (name, created, modified, version)
├── Folders[] (organizational)
└── Entries[]
    ├── PasswordEntry
    │   ├── Title
    │   ├── Username
    │   ├── Password (encrypted)
    │   ├── URL[]
    │   ├── Notes (encrypted)
    │   ├── CustomFields[]
    │   ├── TOTP (encrypted secret)
    │   ├── Attachments[]
    │   └── Metadata (created, modified, history)
    ├── SecureNoteEntry
    ├── CreditCardEntry
    └── IdentityEntry
```

---

## ✅ Tasky

### 1. Value Objects (immutable)
**Testy:**
- [ ] `VaultIdTests` - GUID wrapper s validation
- [ ] `EncryptedStringTests` - ciphertext + nonce container
- [ ] `UrlTests` - normalizace, validace

**Implementace:**
- [ ] `VaultId` - record s GUID interně
- [ ] `EncryptedString` - (ciphertext, nonce, tag)
- [ ] `EncryptedBinary` - pro attachments
- [ ] `Url` - normalizace (vždy https, lowercase host)
- [ ] `Email` - validace formátu

### 2. Entry Types
**Testy:**
- [ ] `PasswordEntryTests` - create, modify, clone
- [ ] `EntryEqualityTests` - value-based equality
- [ ] `EntryValidationTests` - required fields

**Implementace:**
- [ ] `VaultEntryBase` - abstract record:
  ```csharp
  public abstract record VaultEntryBase
  {
      public VaultId Id { get; init; }
      public string Title { get; init; } // LocalizedResourceKey
      public DateTime CreatedAt { get; init; }
      public DateTime ModifiedAt { get; init; }
      public VaultId? FolderId { get; init; }
      public EntryType EntryType { get; init; } // Enum
      public IReadOnlyList<EntryHistoryItem> History { get; init; }
  }
  ```
- [ ] `PasswordEntry` : VaultEntryBase
- [ ] `SecureNoteEntry` : VaultEntryBase
- [ ] `CreditCardEntry` : VaultEntryBase (s maskováním čísla)
- [ ] `IdentityEntry` : VaultEntryBase

**Enums:**
```csharp
public enum EntryType
{
    Password = 1,
    SecureNote = 2,
    CreditCard = 3,
    Identity = 4,
    BankAccount = 5,
    Passport = 6,
    SoftwareLicense = 7
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
    Multiline = 9
}
```

### 3. Vault Container
**Testy:**
- [ ] `VaultTests.Create_New_ReturnsEmpty`
- [ ] `VaultTests.AddEntry_IncreasesCount`
- [ ] `VaultTests.RemoveEntry_ById_Removes`
- [ ] `VaultTests.ModifyEntry_UpdatesTimestamp`

**Implementace:**
- [ ] `Vault` class (aggregate root):
  ```csharp
  public class Vault
  {
      public VaultId Id { get; }
      public VaultHeader Header { get; }
      public VaultMetadata Metadata { get; }
      public IReadOnlyCollection<Folder> Folders { get; }
      public IReadOnlyCollection<VaultEntryBase> Entries { get; }
      
      public Result AddEntry(VaultEntryBase entry);
      public Result RemoveEntry(VaultId id);
      public Result UpdateEntry(VaultEntryBase entry);
      public Maybe<VaultEntryBase> GetEntry(VaultId id);
      public IEnumerable<VaultEntryBase> Search(SearchQuery query);
  }
  ```

### 4. Encrypted Vault Format
**Testy:**
- [ ] `VaultSerializerTests.Serialize_Deserialize_ReturnsEqual`
- [ ] `VaultSerializerTests.Deserialize_WrongPassword_ReturnsError`
- [ ] `VaultSerializerTests.Deserialize_CorruptedData_ReturnsError`
- [ ] `VaultFormatTests.Upgrade_FromV1_ToV2_Works`

**Implementace:**
- [ ] `VaultSerializer` - orchestruje encryption + serialization:
  ```csharp
  public interface IVaultSerializer
  {
      Task<Result<byte[]>> SerializeAsync(Vault vault, MasterKey key, VaultFormatVersion version);
      Task<Result<Vault>> DeserializeAsync(byte[] data, MasterKey key);
      Task<Result<VaultHeader>> ReadHeaderAsync(byte[] data); // bez hesla
  }
  ```
- [ ] `VaultFormatWriter` - zápis do streamu
- [ ] `VaultFormatReader` - čtení ze streamu
- [ ] `VaultFormatMigrator` - upgrade starších verzí

**Binary Format:**
```
[4 bytes] Magic: "PWMN"
[2 bytes] Version (ushort)
[2 bytes] Header Length
[N bytes] Header (JSON, unencrypted):
  - VaultId
  - KdfAlgorithm
  - KdfParameters
  - CipherAlgorithm
  - Salt
  - IV/Nonce
[M bytes] Encrypted Payload (JSON):
  - Metadata
  - Folders[]
  - Entries[]
[16 bytes] Auth Tag (GCM)
```

### 5. Repository Pattern
**Testy:**
- [ ] `VaultRepositoryTests.Save_Load_Roundtrip`
- [ ] `VaultRepositoryTests.Load_NotExists_ReturnsError`
- [ ] `VaultRepositoryTests.Save_Concurrent_Throws`

**Implementace:**
- [ ] `IVaultRepository` interface:
  ```csharp
  public interface IVaultRepository
  {
      Task<bool> ExistsAsync(VaultLocation location);
      Task<Result<Vault>> LoadAsync(VaultLocation location, MasterKey key);
      Task<Result> SaveAsync(VaultLocation location, Vault vault, MasterKey key);
      Task<Result> DeleteAsync(VaultLocation location);
      Task<Result<VaultHeader>> GetHeaderAsync(VaultLocation location);
  }
  ```
- [ ] `FileVaultRepository` - lokální soubor
- [ ] `InMemoryVaultRepository` - pro testy
- [ ] `VaultLocation` - abstraction path/fileId/URL

**Enums:**
```csharp
public enum VaultStorageType
{
    LocalFile = 1,
    Memory = 2,      // Pro testy
    Stream = 3       // Pro import/export
}

public enum VaultRepositoryError
{
    None = 0,
    FileNotFound = 1,
    AccessDenied = 2,
    CorruptedVault = 3,
    WrongPassword = 4,
    VersionTooNew = 5,
    DiskFull = 6
}
```

### 6. Import/Export
**Testy:**
- [ ] `ImportServiceTests.Import_EnpassJson_Success`
- [ ] `ImportServiceTests.Import_BitwardenJson_Success`
- [ ] `ExportServiceTests.Export_Json_IncludesAllData`
- [ ] `ExportServiceTests.Export_Csv_OnlySelectedFields`

**Implementace:**
- [ ] `IImportService` interface
- [ ] `EnpassImporter`
- [ ] `BitwardenImporter`
- [ ] `JsonExporter` (šifrovaný i nešifrovaný)
- [ ] `CsvExporter` (pouze nešifrovaný, s varováním)

**Enums:**
```csharp
public enum ImportFormat
{
    EnpassJson = 1,
    BitwardenJson = 2,
    LastPassCsv = 3,
    ChromeCsv = 4,
    KeePassXml = 5
}

public enum ExportFormat
{
    EncryptedJson = 1,  // Výchozí
    UnencryptedJson = 2, // Varování!
    Csv = 3,             // Varování!
    Pdf = 4              // Emergency kit
}

public enum ExportWarningLevel
{
    None = 0,
    Information = 1,
    Warning = 2,
    Critical = 3  // Nešifrovaný export
}
```

---

## 🎨 UI/UX Specifikace

### Vault Unlock Screen
```
┌─────────────────────────────────────────────┐
│                                             │
│           🔐 Passara                        │
│                                             │
│   ┌─────────────────────────────────────┐   │
│   │                                     │   │
│   │    [Vault Icon] Můj Trezor          │   │
│   │    Naposledy: Dnes 9:30             │   │
│   │                                     │   │
│   └─────────────────────────────────────┘   │
│                                             │
│   Hlavní heslo:                             │
│   ┌─────────────────────────────────────┐   │
│   │ ••••••••••••••••      [👁️]         │   │
│   └─────────────────────────────────────┘   │
│                                             │
│   [  🔓 Odemknout Trezor  ]                │
│                                             │
│   [🔄] Synchronizovat s cloudem            │
│   [⚙️] Nastavení  [?] Nápověda             │
└─────────────────────────────────────────────┘
```

**UX detaily:**
- Focus automaticky na password field při otevření
- Caps Lock indikátor (osobitě pro Windows)
- Progress ring během KDF (jak bylo definováno v Sprint 1)
- Chyby: "Nesprávné heslo" (nikdy neprozrazuj existenci souboru)
- Biometrie: Windows Hello / Touch ID tlačítko vedle hesla

### Import Dialog
```
┌─────────────────────────────────────────────┐
│ 📥 Import hesel                             │
├─────────────────────────────────────────────┤
│                                             │
│ Zdroj: [ Enpass ▼ ]                         │
│                                             │
│ Soubor: [ Vybrat soubor...    ] [📁]        │
│ /home/user/export.json                      │
│                                             │
│ ⚠️ Varování: Importovaný soubor bude        │
│    smazán z disku po úspěšném importu       │
│    z bezpečnostních důvodů.                 │
│                                             │
│ [  ✅ Importovat 122 položek  ]             │
│ [  Zrušit  ]                                │
└─────────────────────────────────────────────┘
```

---

## 📝 Coding Standards

```csharp
// ❌ Špatně:
if (file.Size > 10485760) { ... }

// ✅ Správně:
if (file.Size > VaultConstants.MaxVaultSizeBytes)
{
    return Result.Failure(VaultRepositoryError.VaultTooLarge);
}

// ❌ Špatně:
entry.Type = "password";

// ✅ Správně:
entry = entry with { EntryType = EntryType.Password };

// ❌ Špatně:
return new { success = true, data = vault };

// ✅ Správně:
return Result<Vault>.Success(vault);
```

---

## 📋 Definition of Done

- [ ] Všechny modely immutable (records)
- [ ] 100% branch coverage pro serialization
- [ ] Import/Export testy s reálnými sample daty
- [ ] Performance: Vault s 1000 entries < 100ms load
- [ ] File size: Vault s 1000 entries < 1MB
- [ ] Žádné veřejné setters - vše přes `with` syntax
