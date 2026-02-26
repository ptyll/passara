# Sprint 5: Cloud Sync Engine

**Cíl:** Zero-knowledge synchronizace přes uživatelův cloud (OneDrive, Google Drive, Dropbox, iCloud, Local Folder).

**Délka:** 8-10 dní  
**TDD přístup:** ✅ ANO - nejsložitější část, musí být bulletproof  
**Dependencies:** Sprint 4

---

## 🔄 Sync Architecture

```
[Local Vault] ←→ [Sync Engine] ←→ [Cloud Provider] ←→ [Remote Vault]
                    ↓
            [Conflict Resolution]
            [Delta Compression]
            [Encryption Layer]
```

**Principy:**
1. **Zero-knowledge**: Data jsou šifrována lokálně před uploadem
2. **Delta sync**: Pouze změněné entries, ne celý soubor
3. **Conflict resolution**: Automatický merge nebo uživatelský výběr
4. **Offline-first**: Lokální změny se queue pro sync

---

## ✅ Tasky

### 1. Sync State Management
**Testy:**
- [ ] `SyncStateTests.Merge_NoConflict_ReturnsMerged`
- [ ] `SyncStateTests.Merge_Conflict_CreatesResolution`
- [ ] `SyncStateTests.CalculateDelta_AddedEntry_ReturnsAddOperation`

**Implementace:**
- [ ] `SyncState` record:
  ```csharp
  public record SyncState
  {
      public DateTime LastSyncTimestamp { get; init; }
      public string LastSyncDevice { get; init; }
      public IReadOnlyDictionary<VaultId, EntrySyncMetadata> EntryMetadata { get; init; }
      public string SyncToken { get; init; } // Provider-specific
  }
  ```
- [ ] `EntrySyncMetadata`: hash, timestamp, deleted flag
- [ ] `ISyncStateRepository` - persist sync state locally

**Enums:**
```csharp
public enum SyncStatus
{
    Idle = 0,
    Checking = 1,
    Downloading = 2,
    Uploading = 3,
    ResolvingConflicts = 4,
    Error = 5,
    Disabled = 6
}

public enum SyncConflictResolution
{
    UseLocal = 1,
    UseRemote = 2,
    Merge = 3,
    AskUser = 4
}
```

### 2. Cloud Provider Abstraction
**Testy:**
- [ ] `LocalFolderProviderTests.ListFiles_ReturnsFiles`
- [ ] `LocalFolderProviderTests.Upload_Download_Roundtrip`

**Implementace:**
- [ ] `ICloudStorageProvider` interface (definováno v Sprint 0, nyní implementace):
  ```csharp
  public interface ICloudStorageProvider
  {
      string ProviderName { get; }
      Task<bool> IsAvailableAsync();
      Task<IReadOnlyList<RemoteFile>> ListFilesAsync(string path);
      Task<Stream> DownloadAsync(string fileId);
      Task<string> UploadAsync(string path, Stream data);
      Task DeleteAsync(string fileId);
      Task<DateTime?> GetLastModifiedAsync(string fileId);
  }
  ```
- [ ] `LocalFolderProvider` - MVP pro testování
- [ ] `OneDriveProvider` - Microsoft Graph API
- [ ] `GoogleDriveProvider` - Google Drive API v3
- [ ] `DropboxProvider` - Dropbox API
- [ ] `ICloudProvider` - iCloud Drive (macOS/iOS only)

**Enums:**
```csharp
public enum CloudProviderType
{
    LocalFolder = 1,
    OneDrive = 2,
    GoogleDrive = 3,
    Dropbox = 4,
    ICloud = 5,
    WebDav = 6,    // Pro vlastní NAS
    S3Compatible = 7 // Pro power users
}

public enum SyncDirection
{
    UploadOnly = 1,
    DownloadOnly = 2,
    Bidirectional = 3
}
```

### 3. Delta Sync Algorithm
**Testy:**
- [ ] `DeltaCalculatorTests.Calculate_EntryModified_ReturnsUpdate`
- [ ] `DeltaCalculatorTests.Calculate_EntryDeleted_ReturnsDelete`
- [ ] `DeltaCalculatorTests.Calculate_NoChanges_ReturnsEmpty`

**Implementace:**
- [ ] `DeltaSyncCalculator`:
  ```csharp
  public interface IDeltaSyncCalculator
  {
      DeltaManifest CalculateDelta(
          Vault localVault, 
          SyncState localState,
          SyncState remoteState);
  }
  ```
- [ ] `DeltaManifest` - seznam operací (Add, Update, Delete)
- [ ] `DeltaCompressor` - pokročilé: pouze změněná pole, ne celá entry
- [ ] Binary diff pro attachments (optional - fáze 2)

### 4. Conflict Resolution Engine
**Testy:**
- [ ] `ConflictResolverTests.Resolve_BothModifiedDifferentFields_Merges`
- [ ] `ConflictResolverTests.Resolve_BothModifiedSameField_AsksUser`
- [ ] `ConflictResolverTests.Resolve_BothDeleted_NoOp`

**Implementace:**
- [ ] `IConflictResolver`:
  ```csharp
  public interface IConflictResolver
  {
      ConflictResolutionResult Resolve(
          VaultEntryBase local, 
          VaultEntryBase remote,
          ConflictResolution strategy);
  }
  ```
- [ ] Field-level merge (např. lokálně změněné heslo, remote změněný TOTP)
- [ ] `ConflictResolutionDialogViewModel` pro UI
- [ ] Queue nevyřešených konfliktů

**Enums:**
```csharp
public enum ConflictType
{
    BothModified = 1,
    LocalModifiedRemoteDeleted = 2,
    LocalDeletedRemoteModified = 3,
    AttachmentMismatch = 4
}

public enum MergeStrategy
{
    NewestWins = 1,
    LocalWins = 2,
    RemoteWins = 3,
    FieldLevelMerge = 4
}
```

### 5. Sync Orchestrator
**Testy:**
- [ ] `SyncServiceTests.Sync_NoChanges_NoUpload`
- [ ] `SyncServiceTests.Sync_RemoteNewer_Downloads`
- [ ] `SyncServiceTests.Sync_Conflict_QueuesForResolution`
- [ ] `SyncServiceTests.Sync_NetworkError_RetriesWithBackoff`

**Implementace:**
- [ ] `ISyncService`:
  ```csharp
  public interface ISyncService
  {
      SyncStatus CurrentStatus { get; }
      event EventHandler<SyncProgressEventArgs> ProgressChanged;
      event EventHandler<SyncConflictEventArgs> ConflictDetected;
      
      Task<SyncResult> SyncAsync(SyncOptions options);
      Task ForceUploadAsync(); // Push local jako authoritative
      Task ForceDownloadAsync(); // Pull remote jako authoritative
      void CancelCurrentSync();
  }
  ```
- [ ] `SyncQueue` - offline changes queue
- [ ] `SyncRetryPolicy` - exponential backoff (2^attempt * 1s, max 5 min)
- [ ] `SyncEncryptionLayer` - dodatečná encryption pro cloud storage

### 6. File Format for Sync
**Struktura `.pwmansync` souboru:**
```
[Header - nešifrovaný]
  - Version
  - DeviceId
  - Timestamp
  - ContentHash
  
[Encrypted Payload]
  - VaultData (celý nebo delta)
  - SyncState
  - ConflictMarkers (pokud existují)
```

---

## 🎨 UI/UX Specifikace

### Sync Status Indicator
```
┌─────────────────────────────────────────────────────────────┐
│  Trezor: Můj Trezor                    [✅ Sync OK]        │
│  Poslední sync: Před 2 minutami                             │
└─────────────────────────────────────────────────────────────┘

// Nebo:
│  Trezor: Můj Trezor                    [🔄 Sync... 45%]    │

// Nebo:
│  Trezor: Můj Trezor                    [⚠️ 2 konflikty]    │
```

### Sync Settings Dialog
```
┌─────────────────────────────────────────────────────────────┐
│ ⚙️ Nastavení synchronizace                                  │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Způsob: [ OneDrive ▼ ]                                    │
│                                                             │
│  ✓ Připojeno jako: user@example.com                        │
│                                                             │
│  Složka: /Apps/Passara/                                    │
│  [Změnit složku]                                           │
│                                                             │
│  Automatická synchronizace:                                │
│  (•) Při každé změně (doporučeno)                         │
│  ( ) Každých [ 5 ] minut                                  │
│  ( ) Pouze ručně                                          │
│                                                             │
│  Konflikty:                                                │
│  [ Zeptat se mě ▼ ]                                        │
│  (Použít lokální / Použít cloud / Sloučit automaticky)     │
│                                                             │
│  [  💾 Uložit  ]  [  Odpojit cloud  ]                      │
└─────────────────────────────────────────────────────────────┘
```

### Conflict Resolution Dialog
```
┌─────────────────────────────────────────────────────────────┐
│ ⚠️ Konflikt synchronizace (2/3)                             │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Položka: GitHub                                            │
│  Konflikt: Změněno na obou zařízeních                       │
│                                                             │
│  ┌─────────────────────┬─────────────────────┐             │
│  │  Toto zařízení     │  Cloud (Telefon)   │             │
│  │  14:32 dnes        │  14:35 dnes        │             │
│  ├─────────────────────┼─────────────────────┤             │
│  │  Uživ: dev@em.cz   │  Uživ: NEW@em.cz   │             │
│  │  Heslo: ********   │  Heslo: ********   │             │
│  │  TOTP: 123456      │  TOTP: -           │             │
│  └─────────────────────┴─────────────────────┘             │
│                                                             │
│  [Použít lokální]  [Použít cloud]  [Sloučit]               │
│                                                             │
│  ☑️ Zapamatovat pro tento konflikt                         │
│                                                             │
│  [Další →]                                                  │
└─────────────────────────────────────────────────────────────┘
```

### Sync Progress Toast
```
┌─────────────────────────────────────────────┐
│ 🔄 Synchronizuji...                         │
│ [████████████████████░░░░] 80%             │
│ Stahuji: vault_delta_v3.json               │
└─────────────────────────────────────────────┘
```

---

## 🧪 Testing Strategy

```csharp
// Integration test example
[Fact]
public async Task Sync_TwoClients_ModifySameEntry_Converges()
{
    // Arrange
    var clientA = CreateSyncClient("DeviceA");
    var clientB = CreateSyncClient("DeviceB");
    var cloud = new InMemoryCloudStorage();
    
    // Act - A modifies username, B modifies password
    await clientA.SyncAsync();
    await clientB.SyncAsync();
    
    clientA.ModifyEntry("Github", username: "new@email.com");
    await clientA.SyncAsync();
    
    clientB.ModifyEntry("Github", password: "newpassword123");
    await clientB.SyncAsync();
    
    // Assert - both should have both changes
    var entryA = clientA.GetEntry("Github");
    var entryB = clientB.GetEntry("Github");
    
    entryA.Username.Should().Be("new@email.com");
    entryA.Password.Should().Be("newpassword123");
    entryB.Should().BeEquivalentTo(entryA);
}
```

---

## 📋 Definition of Done

- [ ] Local folder sync funguje 100% spolehlivě
- [ ] OneDrive integrace prochází OAuth flow
- [ ] Conflict resolution UI umožňuje field-level merge
- [ ] Sync přežije výpadek sítě (queue + retry)
- [ ] Delta sync zmenšuje traffic o > 90% oproti full sync
- [ ] Unit testy pro všechny strategie konfliktů
- [ ] Performance: sync < 2s pro vault s 100 entries
