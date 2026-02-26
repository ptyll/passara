# Sprint 0: Project Setup & Architecture Foundation

**Cíl:** Založit solution strukturu, konfigurovat CI/CD pipeline, nastavit TDD infrastrukturu a definovat základní kontrakty.

**Délka:** 3-4 dny  
**TDD přístup:** Ano - nejprve test projekty, pak implementace

---

## 📁 Požadovaná struktura solution

```
Passara/
├── 📁 src/
│   ├── 📁 Passara.Core/              # .NET Standard 2.1
│   ├── 📁 Passara.Desktop/           # Avalonia MVVM
│   ├── 📁 Passara.Desktop.Tests/     # xUnit + FluentAssertions
│   ├── 📁 Passara.NativeHost/        # Browser extension host
│   ├── 📁 Passara.Mobile/            # MAUI
│   ├── 📁 Passara.Mobile.Tests/
│   └── 📁 Passara.BrowserExtension/  # JS/TS (separátní)
├── 📁 tests/
│   ├── 📁 Passara.IntegrationTests/
│   └── 📁 Passara.SecurityTests/     # Penetration testy
├── 📁 docs/
│   ├── 📁 ui-ux/                             # Návrhy, mockupy
│   └── 📁 sprints/                           # Tento soubor
└── 📁 tools/
    └── 📁 BuildScripts/
```

---

## ✅ Tasky (po nejmenších krocích)

### 1. Solution a projekty
- [ ] Vytvořit solution `Passara.sln`
- [ ] Vytvořit `Passara.Core` (.NET Standard 2.1)
- [ ] Vytvořit `Passara.Desktop` (Avalonia MVVM template)
- [ ] Vytvořit test projekty s xUnit, FluentAssertions, Moq
- [ ] Nastavit Directory.Build.props pro shared nastavení
- [ ] Konfigurovat .editorconfig (naming conventions, indentace)

### 2. TDD Infrastructure
- [ ] Nainstalovat NuGet: xUnit, FluentAssertions, Microsoft.NET.Test.Sdk
- [ ] Vytvořit base třídu `UnitTestBase` s common setup
- [ ] Nastavit test naming convention: `[MethodName]_[Scenario]_[ExpectedBehavior]`
- [ ] Konfigurovat code coverage (coverlet + report generator)

### 3. Lokalizace framework
- [ ] Definovat interface `ILocalizationService`
- [ ] Vytvořit Resources.resx strukturu pro Core (en, cs, de)
- [ ] Implementovat `ResourceManagerLocalizationService`
- [ ] Vytvořit extension metody `.L(string key)` pro pohodlné použití

### 4. Enumy a konstanty - Foundation
- [ ] `VaultFormatVersion.cs` - enum pro verze formátu trezoru
- [ ] `KdfAlgorithm.cs` - Argon2id, PBKDF2
- [ ] `EncryptionAlgorithm.cs` - Aes256Gcm, ChaCha20Poly1305
- [ ] `KeyDerivationParameters.cs` - const MemoryCost, Iterations, Parallelism
- [ ] `SyncProviderType.cs` - OneDrive, GoogleDrive, Dropbox, LocalFolder, ICloud

### 5. Base abstrakce a interfaces
- [ ] `IResult<T>` pattern pro operace (Success/Failure s error kódy)
- [ ] `ErrorCode.cs` - enum všech možných chyb (InvalidPassword, VaultCorrupted, SyncFailed...)
- [ ] `IVaultRepository` - interface (zatím prázdný, pro TDD)
- [ ] `ICryptoProvider` - interface (zatím prázdný, pro TDD)

### 6. CI/CD Pipeline
- [ ] GitHub Actions workflow: build + test
- [ ] Code quality gate: min 80% coverage
- [ ] Security scanning: `security-scan` pro .NET
- [ ] Artifact publishing pro desktop app

---

## 🎨 UI/UX Specifikace - Není relevantní pro tento sprint

Tento sprint je infrastrukturní, UI/UX začíná Sprint 3.

**Jediné UX rozhodnutí:** Definovat `AppTheme.cs` enum (Light, Dark, System) pro pozdější implementaci.

---

## 🔒 Security Requirements

- [ ] Povolit `<TreatWarningsAsErrors>` pro security warnings
- [ ] Nastavit `<EnableNETAnalyzers>` a `<AnalysisLevel>latest-Recommended`
- [ ] Zakázat nebezpečné API: `Microsoft.Security.Analyzers`
- [ ] Vytvořit `SecureMemory.cs` - wrapper nad `SecureString` (ikdyž je deprecated, použijeme moderní alternativu)

---

## 📋 Definition of Done

- [ ] Solution buildí bez warnings
- [ ] Všechny testy projektů procházejí (zatím prázdné)
- [ ] Code coverage report generován
- [ ] EditorConfig respektován
- [ ] Žádné hardcoded strings v kódu (vše přes Resources)
- [ ] Všechny magická čísla nahrazena pojmenovanými konstantami

---

## 📝 Příklad kódu (očekávaný standard)

```csharp
// ❌ Špatně:
if (cost == 65536) { ... }

// ✅ Správně:
if (cost == KeyDerivationConstants.Argon2MemoryCost)
{
    logger.LogWarning(ErrorCode.HighMemoryConsumption.L());
}

// ❌ Špatně:
throw new Exception("Password is wrong");

// ✅ Správně:
return Result<VaultKey>.Failure(ErrorCode.InvalidMasterPassword);
```
