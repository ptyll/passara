# Sprint 8: Security Audit & Polish

**Cíl:** Security hardening, penetration testing, performance optimization, release preparation.

**Délka:** 5-7 dní  
**TDD přístup:** ✅ ANO - security testy  
**Dependencies:** Sprint 7

---

## 🔒 Security Audit Checklist

### 1. Cryptographic Audit
**Testy:**
- [ ] `SecurityTests.Keys_AreRandom_NoPatterns`
- [ ] `SecurityTests.Memory_ContainsNoPlaintextAfterDispose`
- [ ] `SecurityTests.Encryption_IsAuthenticated_TamperingDetected`
- [ ] `SecurityTests.Kdf_IsSlow_ResistsBruteForce`

**Audit úkoly:**
- [ ] Ověřit, že se nepoužívá `Random`, pouze `SecureRandom`
- [ ] Ověřit, že se nepoužívá `MD5`, `SHA1`, `DES`, `RC4`
- [ ] Ověřit, že nonce/IV jsou unikátní pro každé šifrování
- [ ] Ověřit, že se neukládá master key v paměti déle než nutné
- [ ] Memory dump analysis: vyhledat plaintext hesla v RAM

### 2. Input Validation
**Audit úkoly:**
- [ ] SQL Injection testy (i když používáme SQLite s parametry)
- [ ] Path Traversal testy (file operations)
- [ ] XXE testy (XML importy)
- [ ] Fuzzing: vstupy 1MB+, speciální znaky, null bytes

### 3. Side-Channel Analysis
**Audit úkoly:**
- [ ] Timing attack testy na porovnávání hesel (constant-time?)
- [ ] Cache-based side channels
- [ ] Power analysis (mobile)

### 4. Network Security
**Audit úkoly:**
- [ ] TLS 1.3 enforcement pro cloud API
- [ ] Certificate pinning pro OneDrive/Google
- [ ] No sensitive data in URLs (logs!)
- [ ] DNS-over-HTTPS podpora (optional)

---

## ✅ Tasky

### 1. Security Hardening
- [ ] `SecurityConstants.cs` - centralizované konstanty:
  ```csharp
  public static class SecurityConstants
  {
      public static readonly TimeSpan MinMasterPasswordTime = TimeSpan.FromMilliseconds(500);
      public static readonly TimeSpan ClipboardClearDelay = TimeSpan.FromSeconds(10);
      public static readonly int MaxFailedUnlockAttempts = 5;
      public static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);
      public static readonly int MinPasswordLength = 12;
  }
  ```
- [ ] `BruteForceProtection` - počítadlo pokusů, lockout:
  ```csharp
  public interface IBruteForceProtection
  {
      Task<AttemptResult> RecordAttemptAsync(string vaultId);
      Task ResetAsync(string vaultId);
      TimeSpan? GetRemainingLockout(string vaultId);
  }
  ```
- [ ] `SecureClipboard` - automatické čištění:
  ```csharp
  public interface ISecureClipboard
  {
      Task CopyAsync(string text, ClearPolicy policy);
      Task ClearAsync();
  }
  
  public enum ClearPolicy
  {
      AfterTimeout = 1,
      OnLock = 2,
      OnExit = 3,
      Never = 4 // Pouze pro testing
  }
  ```

### 2. Vault Lock Behavior
**Enums:**
```csharp
public enum VaultLockTrigger
{
    Manual = 1,
    SystemLock = 2,
    IdleTimeout = 3,
    ClipboardClear = 4,
    AppMinimize = 5,
    AppExit = 6
}

public enum IdleTimeoutDuration
{
    Never = 0,
    OneMinute = 1,
    FiveMinutes = 2,
    FifteenMinutes = 3,
    ThirtyMinutes = 4,
    OneHour = 5
}
```

**Implementace:**
- [ ] `IVaultLockService` - centrální řízení zamykání
- [ ] System lock detection (Windows: WTSRegisterSessionNotification)
- [ ] Idle detection (poslední input)
- [ ] Auto-lock na clipboard clear

### 3. Emergency Access
**Implementace:**
- [ ] `EmergencyAccessKit`:
  - PDF export s master password (šifrovaný?)
  - Recovery codes pro vault access
  - Dědické řešení (dead man's switch)
- [ ] `EmergencyAccessView.axaml` - UI pro generování

### 4. Security Dashboard (Desktop)
```
┌─────────────────────────────────────────────────────────────┐
│ 🔒 Bezpečnostní stav                                        │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Celkové skóre: 94/100 [█████████████░]                     │
│                                                             │
│  ✅ Master heslo: Silné (120 bitů entropie)                │
│  ✅ 2FA povoleno pro 15/42 položek                         │
│  ✅ Žádná duplicitní hesla                                  │
│  ✅ Trezor uzamčen při nečinnosti                           │
│  ✅ Clipboard čištěn po 10s                                 │
│  ✅ Audit log povolen                                       │
│                                                             │
│  ⚠️ Upozornění:                                             │
│     • 3 hesla starší než 1 rok                             │
│     • 1 položka používá slabé šifrování (legacy import)    │
│                                                             │
│  [🔍 Spustit bezpečnostní audit]                           │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### 5. Audit Log
**Implementace:**
- [ ] `IAuditLog`:
  ```csharp
  public interface IAuditLog
  {
      void Log(AuditEventType type, string details);
      IEnumerable<AuditEntry> GetEntries(DateRange range);
      void Export(string path);
  }
  ```
- [ ] `AuditEventType` enum:
  ```csharp
  public enum AuditEventType
  {
      VaultUnlocked = 1,
      VaultLocked = 2,
      EntryCreated = 3,
      EntryModified = 4,
      EntryDeleted = 5,
      EntryViewed = 6,
      PasswordCopied = 7,
      ExportPerformed = 8,
      ImportPerformed = 9,
      SyncCompleted = 10,
      SyncFailed = 11,
      SettingsChanged = 12
  }
  ```
- [ ] Log encryption (šifrován stejným master key)
- [ ] Auto-rotation (keep last 1000 entries)

### 6. Performance Optimization
**Testy:**
- [ ] `PerformanceTests.UnlockVault_1000Entries_Under2Seconds`
- [ ] `PerformanceTests.Search_1000Entries_Under100Ms`
- [ ] `PerformanceTests.EncryptLargeFile_50MB_Under5Seconds`

**Optimization úkoly:**
- [ ] Vault lazy loading (načíst jen metadata, ne celé entries)
- [ ] Search indexing (Lucene.NET nebo vlastní)
- [ ] Thumbnail caching pro attachmenty
- [ ] Database connection pooling

**Performance Targets:**
| Operace | Cíl |
|---------|-----|
| Unlock (< 100 entries) | < 500ms |
| Unlock (1000 entries) | < 2s |
| Search | < 100ms |
| Save entry | < 200ms |
| Sync | < 3s |
| Memory footprint (Desktop) | < 200MB |
| Memory footprint (Mobile) | < 100MB |

### 7. Localization Complete
- [ ] Všechny strings v .resx souborech
- [ ] Jazyky: English (default), Czech, German, French, Spanish
- [ ] RTL support (Arabic, Hebrew) - pro mobilní UI
- [ ] Date/number formáty podle culture

### 8. Accessibility (a11y)
- [ ] Keyboard navigation (Tab order, shortcuts)
- [ ] Screen reader support (AutomationProperties)
- [ ] High contrast theme
- [ ] Font size scaling (100% - 200%)
- [ ] WCAG 2.1 AA compliance

### 9. Documentation
- [ ] `USER_GUIDE.md` - kompletní návod
- [ ] `SECURITY.md` - security model popis
- [ ] `API.md` - pro extension vývojáře
- [ ] Inline help tooltips
- [ ] Onboarding flow (first run)

---

## 🎨 UI Polish

### Micro-interactions
- [ ] Button hover effects (scale 1.02, shadow)
- [ ] Loading skeletons místo spinnerů
- [ ] Success checkmark animation
- [ ] Shake animation pro error
- [ ] Smooth page transitions (200ms)

### Visual Polish
- [ ] Consistent spacing (8px grid)
- [ ] Color contrast ratio > 4.5:1
- [ ] Focus indicators pro keyboard
- [ ] Empty states s ilustracemi
- [ ] Error states s actionable řešením

---

## 📦 Release Preparation

### Code Signing
- [ ] Windows: EV Code Signing Certificate
- [ ] macOS: Developer ID Application
- [ ] iOS: App Store Distribution
- [ ] Android: Play App Signing

### Packaging
- [ ] Windows: MSI installer (WiX) nebo MSIX
- [ ] macOS: .dmg + .pkg
- [ ] Linux: .deb, .rpm, AppImage, Flatpak
- [ ] iOS: App Store Connect
- [ ] Android: Play Console + APK

### Legal
- [ ] EULA (End User License Agreement)
- [ ] Privacy Policy (GDPR compliant)
- [ ] Open source licenses (OSS attribution)
- [ ] Export compliance (cryptography)

---

## 📋 Definition of Done

- [ ] Security audit report bez kritických nálezů
- [ ] Penetration testy proběhly úspěšně
- [ ] Performance testy splňují cíle
- [ ] 100% lokalizace klíčových jazyků
- [ ] Accessibility audit prošel
- [ ] Code signing funguje na všech platformách
- [ ] Release notes připraveny
- [ ] App Store screenshots připraveny
