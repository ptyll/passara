# Sprint 6: Browser Extension & Native Messaging

**Cíl:** Chrome/Edge/Firefox extension s auto-fill, save password, TOTP support přes Native Messaging.

**Délka:** 7-9 dní  
**TDD přístup:** ⚠️ Částečně - JavaScript část manuální testy, C# host unit testy  
**Dependencies:** Sprint 5

---

## 🏗️ Architecture Recap

```
Browser Extension (JS)
├── manifest.json
├── content_script.js (injected do stránek)
├── background.js (service worker)
├── popup.html/ui.js
└── native-messaging.js

Native Host (C# Console App)
├── Program.cs (stdin/stdout loop)
├── MessageProtocol.cs
├── EncryptionLayer.cs (session keys)
└── DesktopIntegration.cs (named pipes)

Desktop App (Avalonia)
└── NativeMessagingServer.cs (named pipe host)
```

---

## ✅ Tasky

### 1. Native Host C# Application
**Testy:**
- [ ] `NativeHostTests.ReadMessage_ValidLength_ReturnsMessage`
- [ ] `NativeHostTests.ReadMessage_InvalidLength_Throws`
- [ ] `NativeHostTests.WriteMessage_FormatsCorrectly`

**Implementace:**
- [ ] `Passara.NativeHost` projekt (Console App)
- [ ] `NativeMessagingProtocol`:
  - Read: 4-byte length prefix + UTF-8 JSON
  - Write: 4-byte length prefix + UTF-8 JSON
- [ ] `MessageHandler` - dispatch podle action:
  ```csharp
  public enum HostAction
  {
      GetCredentials = 1,
      SaveCredentials = 2,
      GeneratePassword = 3,
      CheckStatus = 4,
      FillForm = 5
  }
  ```
- [ ] Named Pipe Client pro komunikaci s Desktop app
- [ ] Logging do %TEMP%/PassaraHost.log (neobsahuje hesla!)

### 2. Native Host Security Layer
**Testy:**
- [ ] `HostEncryptionTests.Handshake_CreatesSessionKey`
- [ ] `HostEncryptionTests.EncryptedMessage_CanDecrypt`

**Implementace:**
- [ ] `HostEncryption`:
  - ECDH handshake pro session key
  - AES-256-GCM pro message encryption
  - Replay attack protection (nonce/timestamp)
- [ ] `OriginValidator` - whitelist domains
- [ ] Extension ID validation

### 3. Browser Manifest (V3)
**Implementace:**
- [ ] `manifest.json`:
  ```json
  {
    "manifest_version": 3,
    "name": "Passara",
    "version": "1.0.0",
    "permissions": [
      "activeTab",
      "scripting",
      "storage",
      "nativeMessaging"
    ],
    "host_permissions": [
      "https://*/*"
    ],
    "background": {
      "service_worker": "background.js"
    },
    "content_scripts": [{
      "matches": ["https://*/*"],
      "js": ["content_script.js"],
      "css": ["content_styles.css"]
    }],
    "action": {
      "default_popup": "popup.html"
    }
  }
  ```

### 4. Content Script (Form Detection)
**Implementace:**
- [ ] `FormDetector.js` - heuristika pro detekci login formulářů:
  - Password input field detection
  - Username/email input detection
  - Submit button detection
  - Form submit interception
- [ ] `FieldIcon.js` - ikona u password fieldu:
  ```
  ┌─────────────────────────────────────────────┐
  │ Username: [                          ]     │
  │ Password: [********          ] [🔒]        │ ← Inline icon
  │            [ Přihlásit ]                    │
  └─────────────────────────────────────────────┘
  ```
- [ ] `AutoFill.js` - vyplnění formuláře (simulace user input)
- [ ] `SavePrompt.js` - detekce nové registrace/změny hesla

### 5. Background Script (Service Worker)
**Implementace:**
- [ ] `NativeMessagingClient.js` - komunikace s hostem
- [ ] `CredentialCache.js` - in-memory cache (neukládat na disk!)
- [ ] `TabStateManager.js` - stav per tab:
  ```javascript
  tabState = {
    url: "https://github.com/login",
    detectedForms: [...],
    filledCredentials: {...},
    lastActivity: timestamp
  }
  ```

### 6. Extension Popup UI
**HTML/CSS/JS:**
```html
<!-- popup.html -->
<div class="popup-container">
  <div class="header">
    <img src="icon48.png" />
    <span>Passara</span>
    <span id="status" class="status-ok">●</span>
  </div>
  
  <div id="locked-view" class="hidden">
    <p>Trezor je uzamčen</p>
    <button id="unlock-btn">Odemknout v aplikaci</button>
  </div>
  
  <div id="entries-list">
    <!-- Dynamicky generováno -->
    <div class="entry" data-id="123">
      <div class="entry-title">GitHub</div>
      <div class="entry-subtitle">developer@email.com</div>
      <div class="entry-actions">
        <button data-action="copy-user">👤</button>
        <button data-action="copy-pass">🔑</button>
        <button data-action="autofill">▶</button>
      </div>
    </div>
  </div>
  
  <div class="footer">
    <button id="generate-btn">Generovat heslo</button>
    <button id="settings-btn">Nastavení</button>
  </div>
</div>
```

### 7. Form Detection Heuristics
**Enums (C# mirror pro konzistenci):**
```csharp
public enum FieldType
{
    Unknown = 0,
    Username = 1,
    Email = 2,
    Password = 3,
    PasswordConfirm = 4,
    CurrentPassword = 5,
    NewPassword = 6,
    Totp = 7,
    Submit = 8
}

public enum FormType
{
    Unknown = 0,
    Login = 1,
    Registration = 2,
    PasswordChange = 3,
    PasswordReset = 4,
    Payment = 5,
    Identity = 6
}
```

**Detekční pravidla (JS):**
```javascript
const PASSWORD_PATTERNS = [
  'type="password"',
  'name="password"',
  'name="passwd"',
  'name="pwd"',
  'id="password"',
  'autocomplete="current-password"',
  'autocomplete="new-password"'
];

const USERNAME_PATTERNS = [
  'type="email"',
  'name="email"',
  'name="username"',
  'name="user"',
  'autocomplete="username"',
  'autocomplete="email"'
];
```

### 8. Desktop Integration Server
**Implementace (Avalonia):**
- [ ] `INativeMessagingServer`:
  ```csharp
  public interface INativeMessagingServer
  {
      void Start();
      void Stop();
      event EventHandler<GetCredentialsRequest> GetCredentialsRequested;
      event EventHandler<SaveCredentialsRequest> SaveCredentialsRequested;
  }
  ```
- [ ] Named Pipe server pro komunikaci s Native Host
- [ ] Request queuing (UI thread safety)
- [ ] Vault unlock prompt pokud je zamčený

---

## 🎨 UI/UX Specifikace

### Inline Menu (Content Script)
```
┌─────────────────────────────────────────────────────────┐
│ Password: [**************] [🔒]                        │
└─────────────────────────────────────────────────────────┘
              ↓ Kliknutí na ikonu
┌─────────────────────────────────────────────────────────┐
│ ┌─────────────────────────────────────────────────────┐ │
│ │ 🔐 Passara                                            │ │
│ ├─────────────────────────────────────────────────────┤ │
│ │ 👤 GitHub - developer@email.com    [Vyplnit]       │ │
│ │ 👤 GitHub - personal@email.com     [Vyplnit]       │ │
│ ├─────────────────────────────────────────────────────┤ │
│ │ [Generovat silné heslo]                              │ │
│ └─────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
```

**Chování:**
- Zobrazí se při kliknutí na ikonu nebo Ctrl+Shift+P
- Vybraná položka se vyplní a odeslá (nebo jen vyplní?)
- Escape zavře menu
- ↑↓ navigace

### Save Password Dialog (Desktop overlay)
```
┌─────────────────────────────────────────────────────────────┐
│ 💾 Uložit heslo?                                            │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Web: github.com                                            │
│  Uživatel: developer@example.com                            │
│  Heslo: ••••••••••••••                                      │
│                                                             │
│  Název položky: [ GitHub                           ]       │
│  Složka:       [ Všechny položky              ▼ ]          │
│                                                             │
│  ☑️ Aktualizovat existující položku                         │
│                                                             │
│  [  💾 Uložit  ]  [  Ignorovat  ]  [  Neposílat znovu  ]   │
└─────────────────────────────────────────────────────────────┘
```

### TOTP Auto-fill
```
Když je detekováno TOTP pole:
1. Extension požádá Desktop o aktuální TOTP kód
2. Desktop vrátí kód + zbývající čas
3. Extension vyplní kód
4. Pokud zbývá < 5s, počká na další kód
```

---

## 🔒 Security Considerations

| Hrozba | Mitigace |
|--------|----------|
| **MITM mezi extension a host** | ECDH handshake, session keys |
| **Malicious website** | Origin whitelist, CSP, sandbox |
| **Clipboard history** | Secure clipboard API, auto-clear |
| **Page scripts reading data** | Shadow DOM pro UI, isolated content script |
| **Extension compromised** | Minimal permissions, no storage of secrets |

---

## 🧪 Testing

```csharp
// C# Native Host test
[Fact]
public void HandleMessage_GetCredentials_ReturnsEncryptedResponse()
{
    var handler = new MessageHandler(mockDesktopConnection);
    var request = new GetCredentialsRequest 
    { 
        Url = "https://github.com/login",
        Action = HostAction.GetCredentials 
    };
    
    var response = handler.Handle(request);
    
    response.Should().NotBeNull();
    response.Entries.Should().NotBeEmpty();
    response.Entries.First().Password.Should().BeEncrypted();
}
```

```javascript
// JS Form Detection test (manual)
// Test on: github.com/login, gmail.com, facebook.com, etc.
const forms = FormDetector.detectForms(document);
console.assert(forms.length > 0, 'Should detect login form');
console.assert(forms[0].passwordField, 'Should find password input');
```

---

## 📋 Definition of Done

- [ ] Native Host registrován v Chrome, Edge, Firefox
- [ ] Form detection funguje na top 50 webů
- [ ] Auto-fill vyplní username + password + submit
- [ ] Save prompt se zobrazí po registraci
- [ ] TOTP auto-fill funguje
- [ ] Extension neukládá žádná data lokálně (vše přes host)
- [ ] Manifest V3 pro Chrome/Edge, V2 pro Firefox
- [ ] Content Security Policy striktní
