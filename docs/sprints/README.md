# Passara - Implementation Sprints

Tento adresář obsahuje detailní implementační plán pro Passara aplikaci.

## 🎯 Cíl projektu

Vytvořit moderního, bezpečného a uživatelsky přívětivého správce hesel, který konkuruje komerčním řešením (Bitwarden, 1Password, Enpass) s důrazem na:

- **Zero-knowledge architekturu** - vše šifrováno lokálně
- **Cross-platform** - Windows, macOS, Linux, iOS, Android
- **User-owned cloud** - žádné vlastní servery, sync přes OneDrive/Google Drive/Dropbox
- **Výjimečný UX** - rychlý, intuitivní, keyboard-first
- **Open source** - možnost auditovat kód

## 🏗️ Architektura

```
┌─────────────────────────────────────────────────────────────────┐
│                         PRESENTATION LAYER                       │
├─────────────────────────────────────────────────────────────────┤
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐  │
│  │   Desktop    │  │    Mobile    │  │  Browser Extension   │  │
│  │  (Avalonia)  │  │    (MAUI)    │  │   (Chrome/Firefox)   │  │
│  └──────┬───────┘  └──────┬───────┘  └──────────┬───────────┘  │
│         │                 │                      │              │
│         └────────┬────────┴──────────┬───────────┘              │
│                  │                   │                          │
│         ┌────────▼────────┐  ┌───────▼──────┐                   │
│         │  Native Host    │  │  Native API  │                   │
│         │  (Named Pipe)   │  │  (Keychain)  │                   │
│         └─────────────────┘  └──────────────┘                   │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                         APPLICATION LAYER                        │
├─────────────────────────────────────────────────────────────────┤
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐  │
│  │    Vault     │  │     Sync     │  │   Password Generator │  │
│  │   Service    │  │   Engine     │  │       Service        │  │
│  └──────────────┘  └──────────────┘  └──────────────────────┘  │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐  │
│  │   Auto-Type  │  │   TOTP       │  │   Import/Export      │  │
│  │   Service    │  │   Service    │  │      Service         │  │
│  └──────────────┘  └──────────────┘  └──────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                           CORE LAYER                             │
├─────────────────────────────────────────────────────────────────┤
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐  │
│  │ Cryptography │  │    Vault     │  │   Sync Abstraction   │  │
│  │   (libsodium)│  │   Models     │  │                      │  │
│  └──────────────┘  └──────────────┘  └──────────────────────┘  │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐  │
│  │    KDF       │  │  Encryption  │  │   Secure Random      │  │
│  │ (Argon2id)   │  │  (AES-GCM)   │  │                      │  │
│  └──────────────┘  └──────────────┘  └──────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                        INFRASTRUCTURE LAYER                      │
├─────────────────────────────────────────────────────────────────┤
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐  │
│  │    SQLite    │  │  Cloud APIs  │  │  Secure Storage      │  │
│  │  (SQLCipher) │  │(OneDrive/GDrive)│  │  (Platform-specific) │  │
│  └──────────────┘  └──────────────┘  └──────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

## 📅 Sprinty

| Sprint | Téma | Délka | Klíčové výstupy |
|--------|------|-------|-----------------|
| [Sprint 0](./Sprint_0_Project_Setup.md) | Project Setup & Architecture | 3-4 dny | Solution structure, CI/CD, TDD infra |
| [Sprint 1](./Sprint_1_Core_Cryptography.md) | Core Cryptography | 5-7 dní | Argon2id, AES-GCM, SecureRandom |
| [Sprint 2](./Sprint_2_Data_Layer.md) | Data Layer & Models | 5-6 dní | Vault format, Repository pattern |
| [Sprint 3](./Sprint_3_Desktop_UI_Foundation.md) | Desktop UI Foundation | 7-10 dní | Avalonia UI, MVVM, Navigation |
| [Sprint 4](./Sprint_4_Desktop_Features.md) | Desktop Features | 7-9 dní | TOTP, Generator, Auto-Type |
| [Sprint 5](./Sprint_5_Cloud_Sync.md) | Cloud Sync Engine | 8-10 dní | OneDrive/GDrive, Delta sync |
| [Sprint 6](./Sprint_6_Browser_Extension.md) | Browser Extension | 7-9 dní | Native Messaging, Form detection |
| [Sprint 7](./Sprint_7_Mobile_App.md) | Mobile App | 8-10 dní | MAUI, Biometrie, iOS/Android |
| [Sprint 8](./Sprint_8_Security_Audit.md) | Security & Polish | 5-7 dní | Audit, Performance, Release |

**Celkový odhad:** 56-72 dní (11-14 týdnů) pro full-featured aplikaci

## 🛠️ Tech Stack

| Kategorie | Technologie |
|-----------|-------------|
| **Desktop UI** | Avalonia UI 11.x |
| **Mobile UI** | .NET MAUI 8.x |
| **Core** | .NET Standard 2.1 |
| **Crypto** | libsodium (Sodium.Core) |
| **Database** | SQLite + SQLCipher |
| **Testing** | xUnit, FluentAssertions, Moq |
| **Sync** | OneDrive API, Google Drive API |
| **Extension** | JavaScript, Native Messaging |

## 🎨 Design Principles

### UI/UX
- **Keyboard First** - všechny operace dostupné bez myši
- **Quick Search** - global hotkey (Ctrl+Shift+P)
- **Dark/Light Theme** - automatická detekce systému
- **Contextual Actions** - swipe, right-click menu
- **Progressive Disclosure** - pokročilé funkce skryté

### Security
- **Zero Knowledge** - nikdy neposíláme plaintext na server
- **Memory Safety** - SecureBuffer, zeroizace
- **Defense in Depth** - více vrstev ochrany
- **Fail Secure** - při chybě zamknout vault

### Code Quality
- **TDD** - testy před implementací
- **No Magic Numbers** - všechny konstanty pojmenované
- **No Hardcoded Strings** - lokalizace od začátku
- **Immutable Models** - records pro doménové objekty

## 🚀 Getting Started

1. **Sprint 0** - Clone repo, setup environment
2. **Sprint 1-2** - Implement Core a Data layer (základ)
3. **Sprint 3-4** - Desktop MVP (usable app)
4. **Sprint 5** - Přidat Sync (multi-device)
5. **Sprint 6** - Browser extension (productivity)
6. **Sprint 7** - Mobile app (ubiquity)
7. **Sprint 8** - Security audit a release

## 📋 Definition of Done (Global)

Každý sprint je považován za dokončený, když:

- [ ] Všechny tasky splněny
- [ ] Test coverage > 80%
- [ ] Žádné kritické security issues
- [ ] UI/UX review schválen
- [ ] Documentation aktualizována
- [ ] Code review dokončeno

## 🔐 Security Model

```
User Master Password
       ↓
   Argon2id (KDF)
       ↓
   Encryption Key
       ↓
   ┌──────────────┐
   │  Vault Data  │ ← Šifrováno AES-256-GCM
   │  (JSON)      │
   └──────────────┘
       ↓
   Upload to Cloud
       ↓
   Encrypted Blob
   (Zero Knowledge)
```

## 📄 License

TBD - doporučeno AGPL-3.0 nebo MIT dle preference

---

**Poslední aktualizace:** 2026-02-26  
**Verze dokumentace:** 1.0
