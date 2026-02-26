# Sprint 4: Desktop Features

**Cíl:** TOTP, Password Generator, Auto-Type, Entry History, Attachments.

**Délka:** 7-9 dní  
**TDD přístup:** ✅ ANO  
**Dependencies:** Sprint 3

---

## ✅ Tasky

### 1. TOTP (Time-based One-Time Password)

**Testy:**
- [ ] `TotpServiceTests.GenerateCode_ValidSecret_Returns6Digits`
- [ ] `TotpServiceTests.GenerateCode_SameTimeWindow_ReturnsSameCode`
- [ ] `TotpServiceTests.ValidateCode_Correct_ReturnsTrue`
- [ ] `TotpServiceTests.ValidateCode_TolerantToDrift`

**Implementace:**
- [ ] `ITotpService` interface:
  ```csharp
  public interface ITotpService
  {
      TotpCode GenerateCode(byte[] secret, DateTime timestamp);
      bool ValidateCode(byte[] secret, string code, int toleranceSteps = 1);
      int GetRemainingSeconds(DateTime timestamp); // Pro progress bar
  }
  ```
- [ ] `TotpCode` record: `string Code`, `int RemainingSeconds`, `int Period`
- [ ] `TotpUriParser` - `otpauth://` formát podpora
- [ ] `QrCodeScannerService` - pro desktop (camera nebo screen capture)

**Enums:**
```csharp
public enum TotpAlgorithm
{
    Sha1 = 1,    // Legacy, bohužel stále běžné
    Sha256 = 2,
    Sha512 = 3
}

public static class TotpConstants
{
    public const int DefaultPeriodSeconds = 30;
    public const int DefaultDigits = 6;
    public const int MaxDriftSteps = 2; // ±1 period toleranc
}
```

**UI/UX:**
```
┌─────────────────────────────────────────────┐
│ 🔢 TOTP                                     │
│ ┌─────────────────────────────────────────┐ │
│ │ 123 456    [⏱️██████░░░░ 18s] [📋]     │ │
│ └─────────────────────────────────────────┘ │
│                                             │
│ Tajný klíč: •••••••••••••••• [👁️]         │
│                                             │
│ [Nastavit z QR kódu] [Zadat ručně]         │
└─────────────────────────────────────────────┘
```

- Progress bar změní barvu (zelená → žlutá < 5s → červená < 2s)
- Auto-copy při 2s? Ne, to je riskantní - raději tlačítko
- Klávesová zkratka pro copy TOTP (např. Ctrl+Shift+T)

---

### 2. Password Generator

**Testy:**
- [ ] `PasswordGeneratorTests.Generate_RespectsLength`
- [ ] `PasswordGeneratorTests.Generate_IncludeAllCharsets_HasAtLeastOne`
- [ ] `PasswordGeneratorTests.Generate_ExcludeAmbiguous_NoSimilarChars`
- [ ] `PasswordGeneratorTests.CalculateEntropy_ReturnsCorrectBits`

**Implementace:**
- [ ] `IPasswordGenerator` (extend z Core):
  - `PasswordGenerationOptions options`
- [ ] `PasswordStrengthEvaluator`:
  - Zxcvbn.NET integrace nebo vlastní evaluace
  - Entropie výpočet
- [ ] `PasswordGenerationOptions`:
  ```csharp
  public record PasswordGenerationOptions
  {
      public int Length { get; init; }
      public PasswordCharacterSet CharacterSets { get; init; }
      public bool ExcludeAmbiguous { get; init; }
      public bool RequireAtLeastOneOfEach { get; init; }
      public int MinNumbers { get; init; }
      public int MinSpecial { get; init; }
  }
  ```

**Enums:**
```csharp
public enum PasswordStrength
{
    VeryWeak = 1,    // < 40 bits
    Weak = 2,        // 40-60 bits
    Fair = 3,        // 60-80 bits
    Strong = 4,      // 80-120 bits
    VeryStrong = 5   // > 120 bits
}

public static class PasswordGenerationPresets
{
    public const int LengthRandom = 16;
    public const int LengthPassphrase = 4; // words
    public const int LengthPin = 6;
    public const int LengthMax = 128;
}
```

**UI/UX - Generator Window:**
```
┌─────────────────────────────────────────────────────────────┐
│ 🔧 Generátor hesla                                          │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Délka: [━━━●━━━━━━━━] 20                                   │
│                                                             │
│  ☑️ Velká písmena (A-Z)                                     │
│  ☑️ Malá písmena (a-z)                                      │
│  ☑️ Číslice (0-9)                                           │
│  ☑️ Speciální znaky (!@#$%^&*)                              │
│  ☐ Mezery (pro passphrase)                                  │
│  ☑️ Vyloučit nejednoznačné (0 vs O, 1 vs l)                 │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ Tr0ub4dor&3!Ex@mple#9                              │    │
│  │ [🔄] [📋]                                          │    │
│  └─────────────────────────────────────────────────────┘    │
│                                                             │
│  Síla: [████████████░░] 95 bitů - Velmi silné               │
│  Odhadovaný čas k prolomení: 3 roky                         │
│                                                             │
│  [✅ Použít toto heslo]  [Generovat další]                  │
└─────────────────────────────────────────────────────────────┘
```

**UX detaily:**
- Passphrase mód (diceware): "correct-horse-battery-staple"
- PIN mód: pouze čísla
- Historie posledních 10 generovaných (pro undo)
- "Type": Random, Memorable, PIN, WPA Key

---

### 3. Auto-Type Engine

**Testy:**
- [ ] `AutoTypeServiceTests.TypeSequence_SendsKeystrokes`
- [ ] `AutoTypeServiceTests.ParseTemplate_ReplacesPlaceholders`
- [ ] `AutoTypeServiceTests.ClearClipboard_AfterTimeout`

**Implementace:**
- [ ] `IAutoTypeService`:
  ```csharp
  public interface IAutoTypeService
  {
      Task TypeSequenceAsync(string sequence, Window targetWindow);
      Task AutoTypeEntryAsync(VaultEntryBase entry, AutoTypeSequenceType sequenceType);
  }
  ```
- [ ] `AutoTypeParser` - placeholder syntax:
  - `{USERNAME}` - uživatelské jméno
  - `{PASSWORD}` - heslo
  - `{TOTP}` - aktuální TOTP kód
  - `{ENTER}`, `{TAB}`, `{DELAY 500}` - speciální klávesy
- [ ] Platform-specific:
  - Windows: `SendInput` API
  - Linux: `xdotool` nebo `libevdev`
  - macOS: `CGEventPost`

**Enums:**
```csharp
public enum AutoTypeSequenceType
{
    UsernameOnly = 1,
    PasswordOnly = 2,
    UsernameThenTabThenPassword = 3,
    UsernameThenTabThenPasswordThenEnter = 4, // Default
    Custom = 5
}

public enum AutoTypeDelay
{
    Default = 10,     // ms mezi klávesami
    AfterFocus = 100, // ms po focus okna
    AfterField = 50   // ms mezi poli
}
```

**UI/UX:**
- V entry detailu: "Auto-Type Sequence" field s presety
- Global hotkey: Ctrl+Shift+A → vybere poslední použité entry nebo ukáže quick search
- Match window title/URL automaticky
- Window matching:
  ```
  Title: "GitHub - Login" → Match: "*github*login*"
  URL: "https://github.com/login" → Match: "github.com/login"
  ```

---

### 4. Entry History & Versioning

**Testy:**
- [ ] `EntryHistoryTests.CreateSnapshot_PreservesState`
- [ ] `EntryHistoryTests.Restore_RestoresPreviousState`
- [ ] `EntryHistoryTests.MaxHistorySize_Respected`

**Implementace:**
- [ ] `IEntryHistoryService`:
  ```csharp
  public interface IEntryHistoryService
  {
      void CreateSnapshot(VaultEntryBase entry);
      Maybe<VaultEntryBase> GetPreviousVersion(VaultId entryId, int stepsBack);
      IEnumerable<VaultEntryBase> GetHistory(VaultId entryId);
      void RestoreVersion(VaultId entryId, DateTime timestamp);
      void CleanupOldVersions(int keepCount);
  }
  ```

**Enums:**
```csharp
public enum HistoryRetentionPolicy
{
    KeepLast10 = 1,
    KeepLast30 = 2,
    KeepLast100 = 3,
    KeepForever = 4,
    KeepForDays30 = 5
}
```

**UI/UX - History Dialog:**
```
┌─────────────────────────────────────────────────────────────┐
│ 📜 Historie změn - GitHub                                   │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  [Dnes 14:30]  Změněno heslo              [Obnovit]        │
│  [Včera 9:15] Přidáno TOTP                [Obnovit]        │
│  [Před 3 dny] Vytvořena položka           [Obnovit]        │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ Porovnání:                                          │    │
│  │ Aktuální        |  Historie (14:30)                 │    │
│  │ ────────────────|──────────────────                   │    │
│  │ *************** | •••••••••••••••                   │    │
│  │ [Původní je maskováno, klikněte pro odhalení]       │    │
│  └─────────────────────────────────────────────────────┘    │
│                                                             │
│  [Zavřít]                                                   │
└─────────────────────────────────────────────────────────────┘
```

---

### 5. Attachments

**Testy:**
- [ ] `AttachmentServiceTests.AddAttachment_IncreasesSize`
- [ ] `AttachmentServiceTests.EncryptAttachment_ReturnsEncryptedStream`

**Implementace:**
- [ ] `Attachment` record:
  ```csharp
  public record Attachment
  {
      public Guid Id { get; init; }
      public string FileName { get; init; }
      public long SizeBytes { get; init; }
      public string ContentType { get; init; }
      public EncryptedBinary EncryptedData { get; init; }
      public DateTime AddedAt { get; init; }
  }
  ```
- [ ] `IAttachmentService` - správa příloh
- [ ] Streaming encryption pro velké soubory (chunked AES-GCM)
- [ ] Quota management (max 100MB na vault např.)

**Enums:**
```csharp
public enum AttachmentError
{
    None = 0,
    FileTooLarge = 1,
    QuotaExceeded = 2,
    InvalidType = 3,
    EncryptionFailed = 4
}

public static class AttachmentLimits
{
    public const long MaxFileSize = 50 * 1024 * 1024; // 50 MB
    public const long MaxTotalSize = 500 * 1024 * 1024; // 500 MB
    public const int MaxFilesPerEntry = 10;
}
```

**UI/UX:**
```
┌─────────────────────────────────────────────┐
│ 📎 Přílohy (3)                              │
├─────────────────────────────────────────────┤
│ ┌─────────────────────────────────────────┐ │
│ │ 📄 passport.pdf          2.4 MB  [🗑️] │ │
│ │ 🖼️ screenshot.png      450 KB  [🗑️] │ │
│ │ 📊 config.json           12 KB  [🗑️] │ │
│ └─────────────────────────────────────────┘ │
│ Celková velikost: 2.9 MB / 500 MB           │
│                                             │
│ [📎 Přidat přílohu]                         │
└─────────────────────────────────────────────┘
```

---

### 6. Security Audit Dashboard

**UI/UX:**
```
┌─────────────────────────────────────────────────────────────┐
│ 🔒 Bezpečnostní audit                                       │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Celkové skóre: 87/100 [███████████░]                       │
│                                                             │
│  ⚠️ Problémy k řešení:                                      │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ 🔴 3 hesla jsou slabá (< 40 bitů)                   │    │
│  │    [Zobrazit] [Hromadně změnit]                     │    │
│  ├─────────────────────────────────────────────────────┤    │
│  │ 🟡 12 hesel se opakuje v jiných službách            │    │
│  │    [Zobrazit]                                       │    │
│  ├─────────────────────────────────────────────────────┤    │
│  │ 🟡 2 položky nemají 2FA                             │    │
│  │    [Doporučit povolení 2FA]                         │    │
│  └─────────────────────────────────────────────────────┘    │
│                                                             │
│  ✅ Dobře:                                                  │
│     • Všechny hesla jsou unikátní v rámci trezoru          │
│     • Průměrná entropie: 85 bitů                           │
│     • Žádné kompromitované hesla (HaveIBeenPwned check)    │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 📋 Definition of Done

- [ ] TOTP funguje s Google Auth, Microsoft Auth, GitHub
- [ ] Password generator má entropii > 100 bitů defaultně
- [ ] Auto-Type funguje v Chrome, Edge, Firefox (Windows)
- [ ] Entry history ukládá max 10 verzí (configurovatelné)
- [ ] Attachments šifrovány samostatně, streaming pro > 1MB
- [ ] Všechny features pokryty UI testy (Avalonia.Headless)
