# Sprint 3: Desktop UI Foundation (Avalonia)

**Cíl:** Moderní, responzivní UI s MVVM, navigation, theming a keyboard-first UX.

**Délka:** 7-10 dní  
**TDD přístup:** ✅ ANO - ViewModel testy  
**Dependencies:** Sprint 2

---

## 🎨 Design System

### Color Palette
```xml
<!-- Light Theme -->
<Color x:Key="BackgroundPrimary">#FFFFFF</Color>
<Color x:Key="BackgroundSecondary">#F5F5F5</Color>
<Color x:Key="BackgroundTertiary">#E8E8E8</Color>
<Color x:Key="ForegroundPrimary">#1A1A1A</Color>
<Color x:Key="ForegroundSecondary">#6B6B6B</Color>
<Color x:Key="AccentPrimary">#2563EB</Color>      <!-- Blue 600 -->
<Color x:Key="AccentSecondary">#3B82F6</Color>    <!-- Blue 500 -->
<Color x:Key="Success">#10B981</Color>            <!-- Emerald 500 -->
<Color x:Key="Warning">#F59E0B</Color>            <!-- Amber 500 -->
<Color x:Key="Error">#EF4444</Color>              <!-- Red 500 -->
<Color x:Key="SecurityStrong">#10B981</Color>
<Color x:Key="SecurityMedium">#F59E0B</Color>
<Color x:Key="SecurityWeak">#EF4444</Color>

<!-- Dark Theme -->
<Color x:Key="BackgroundPrimary">#0F0F0F</Color>
<Color x:Key="BackgroundSecondary">#1A1A1A</Color>
<Color x:Key="BackgroundTertiary">#262626</Color>
<Color x:Key="ForegroundPrimary">#FAFAFA</Color>
<Color x:Key="ForegroundSecondary">#A3A3A3</Color>
```

### Typography
```xml
<FontFamily x:Key="FontFamilyBase">Inter, Segoe UI, sans-serif</FontFamily>
<FontFamily x:Key="FontFamilyMono">JetBrains Mono, Consolas, monospace</FontFamily>

<x:Double x:Key="FontSizeXs">12</x:Double>
<x:Double x:Key="FontSizeSm">14</x:Double>
<x:Double x:Key="FontSizeBase">16</x:Double>
<x:Double x:Key="FontSizeLg">18</x:Double>
<x:Double x:Key="FontSizeXl">24</x:Double>
<x:Double x:Key="FontSize2Xl">32</x:Double>
```

### Spacing System (4px base)
```xml
<x:Double x:Key="Space1">4</x:Double>
<x:Double x:Key="Space2">8</x:Double>
<x:Double x:Key="Space3">12</x:Double>
<x:Double x:Key="Space4">16</x:Double>
<x:Double x:Key="Space6">24</x:Double>
<x:Double x:Key="Space8">32</x:Double>
```

---

## ✅ Tasky

### 1. Theming Infrastructure
**Testy:**
- [ ] `ThemeServiceTests.SetTheme_PersistsToSettings`
- [ ] `ThemeServiceTests.SystemThemeChange_UpdatesUi`

**Implementace:**
- [ ] `IThemeService` interface
- [ ] `ThemeType` enum: Light, Dark, System
- [ ] `ThemeManager` - dynamic resource switching
- [ ] `FluentTheme` + custom overrides

### 2. Navigation Framework
**Testy:**
- [ ] `NavigationServiceTests.NavigateTo_ChangesCurrentPage`
- [ ] `NavigationServiceTests.GoBack_ReturnsToPrevious`

**Implementace:**
- [ ] `INavigationService` - region-based navigation
- [ ] `ViewType` enum: Unlock, Vault, Settings, Generator, etc.
- [ ] `NavigationStack` - history management
- [ ] `ShellViewModel` - main container

### 3. Shell Layout (Hlavní okno)
```
┌─────────────────────────────────────────────────────────────┐
│ TitleBar (custom, acrylic)                           [_][□][X]│
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────┐  ┌──────────────────────────────────────────┐ │
│  │          │  │                                          │ │
│  │  [🔍]    │  │  [Detail View - Password Entry]          │ │
│  │  Search  │  │                                          │ │
│  │          │  │  Title: GitHub                           │ │
│  │ Folders  │  │  ┌──────────────────────────────────┐    │ │
│  │ ─────────│  │  │ 👤 Username: developer@email.com │    │ │
│  │ 📁 All   │  │  │ 🔑 Password: •••••••• [👁️][📋]  │    │ │
│  │ ⭐ Fav   │  │  │ 🔢 TOTP: 123456 [⏱️ 25s]         │    │ │
│  │ 🗂️ Work │  │  │ 🌐 URL: github.com [🌍]          │    │ │
│  │ 🏠 Home  │  │  └──────────────────────────────────┘    │ │
│  │          │  │                                          │ │
│  │ [+] Add  │  │  [Edit] [Delete] [History]               │ │
│  │          │  │                                          │ │
│  └──────────┘  └──────────────────────────────────────────┘ │
│                                                             │
│  [🔒 Lock] [⚙️] [👤]                              Status: ✓ │
└─────────────────────────────────────────────────────────────┘
```

**Implementace:**
- [ ] `ShellView.axaml` - hlavní okno
- [ ] `SidebarView.axaml` - levý panel
- [ ] `ContentPane` - dynamický obsah
- [ ] `StatusBar` - sync status, lock button

### 4. Unlock ViewModel & View
**Testy:**
- [ ] `UnlockViewModelTests.UnlockCommand_ValidPassword_NavigatesToVault`
- [ ] `UnlockViewModelTests.UnlockCommand_InvalidPassword_ShowsError`
- [ ] `UnlockViewModelTests.BiometricAvailable_ShowsBiometricButton`

**Implementace:**
- [ ] `UnlockViewModel`:
  - `SecureString PasswordInput` (binding one-way)
  - `ICommand UnlockCommand` (async)
  - `ICommand BiometricUnlockCommand`
  - `bool IsBiometricAvailable`
  - `string SelectedVaultName`
  - `ICommand SelectDifferentVaultCommand`
- [ ] `UnlockView.axaml` - clean, centered design

### 5. Vault Browser (List + Search)
**Testy:**
- [ ] `VaultBrowserViewModelTests.Search_UpdatesFilteredEntries`
- [ ] `VaultBrowserViewModelTests.SelectEntry_NavigatesToDetail`

**Implementace:**
- [ ] `VaultBrowserViewModel`:
  - `ObservableCollection<EntryViewModel> Entries`
  - `string SearchQuery` (reactive, debounced 200ms)
  - `EntryViewModel SelectedEntry`
  - `ICommand CopyUsernameCommand`
  - `ICommand CopyPasswordCommand`
  - `ICommand CopyTotpCommand`
- [ ] `EntryViewModel` - wrapper pro VaultEntryBase s UI-specific props
- [ ] `EntryListView.axaml` - VirtualizingStackPanel pro výkon

### 6. Entry Detail View
**Implementace:**
- [ ] `EntryDetailViewModel`:
  - `EntryViewModel Entry`
  - `bool IsEditing`
  - `ICommand EditCommand`
  - `ICommand SaveCommand`
  - `ICommand CancelCommand`
  - `ICommand DeleteCommand`
  - `ICommand ShowHistoryCommand`
- [ ] `EntryDetailView.axaml`:
  - Read-only mód: ikony, copy tlačítka
  - Edit mód: TextBoxy, ComboBoxy
  - FieldType-specific controly

### 7. Global Hotkeys
**Testy:**
- [ ] `HotkeyServiceTests.Register_QuickSearch_Success`

**Implementace:**
- [ ] `IGlobalHotkeyService`:
  - `Hotkey QuickSearch` - Ctrl+Shift+P (konfigurovatelné)
  - `Hotkey AutoType` - Ctrl+Shift+A
  - `Hotkey LockVault` - Ctrl+L
- [ ] Platform-specific implementace (Windows: RegisterHotKey API)

**Enums:**
```csharp
public enum HotkeyAction
{
    QuickSearch = 1,
    AutoType = 2,
    LockVault = 3,
    CopyPassword = 4,
    CopyUsername = 5
}

public enum ModifierKey
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Windows = 8
}
```

### 8. Toast Notifications
**Implementace:**
- [ ] `IToastService`:
  - `ShowSuccess(string message)`
  - `ShowError(string message)`
  - `ShowWarning(string message)`
  - `ShowInfo(string message)`
- [ ] Toast positioning: bottom-right
- [ ] Auto-dismiss: 3s pro success/info, 8s pro error

---

## 🎨 UI/UX Detaily

### Quick Search Overlay (Global Hotkey)
```
┌─────────────────────────────────────────────┐
│ 🔍 Vyhledat položku...                      │
├─────────────────────────────────────────────┤
│ ┌─────────────────────────────────────────┐ │
│ │ github      [⌨️ Ctrl+Enter: Auto-type] │ │
│ │ GitHub - osobní účet                    │ │
│ │ 👤 developer@email.com                  │ │
│ └─────────────────────────────────────────┘ │
│ ┌─────────────────────────────────────────┐ │
│ │ gmail                                     │
│ │ Google Mail - pracovní                  │ │
│ │ 👤 admin@company.com                    │ │
│ └─────────────────────────────────────────┘ │
│                                             │
│ [←/→] Kopírovat uživatel/heslo              │
│ [Enter] Auto-type                           │
│ [Esc] Zavřít                                │
└─────────────────────────────────────────────┘
```

**UX:**
- Fuzzy search (např. "gml" najde "gmail")
- Keyboard-only navigation (↑↓ Enter)
- Instant preview
- Poslední použité položky nahoře

### Password Field s Revealem
```
┌─────────────────────────────────────────────┐
│ 🔑 Heslo                                    │
│ ┌─────────────────────────────────────────┐ │
│ │ ••••••••••••••••    [👁️] [📋] [🔄]    │ │
│ └─────────────────────────────────────────┘ │
│ Entropie: [████████████░░] 85 bitů (Silné) │
└─────────────────────────────────────────────┘
```

**Chování:**
- 👁️ - Hold to reveal (mouse down = show, up = hide)
- 📋 - Copy to clipboard (auto-clear za 10s)
- 🔄 - Generate new password

### Auto-Type Progress
```
┌─────────────────────────────────────────────┐
│ Auto-type aktivní...                        │
│                                             │
│ Zapisuji: github.com                        │
│ Postup: Uživatel ▶ Heslo ▶ Enter            │
│                                             │
│ [ Zrušit  (Esc) ]                           │
└─────────────────────────────────────────────┘
```

---

## 📝 MVVM Standards

```csharp
// ViewModel base
public abstract class ViewModelBase : ObservableObject, IDisposable
{
    protected readonly ILocalizationService L;
    protected readonly ILogger Logger;
    
    protected ViewModelBase(ILocalizationService localization, ILogger logger)
    {
        L = localization;
        Logger = logger;
    }
    
    public abstract void Dispose();
}

// Command creation
public ICommand UnlockCommand => new AsyncRelayCommand(
    ExecuteUnlockAsync, 
    () => !IsUnlocking && PasswordLength >= PasswordPolicy.MinLength);

// Never expose domain models directly
private EntryViewModel MapToViewModel(VaultEntryBase entry) => new()
{
    Id = entry.Id,
    Title = entry.Title,
    DisplayTitle = string.IsNullOrEmpty(entry.Title) 
        ? L[ResourceKeys.UntitledEntry] 
        : entry.Title,
    Icon = GetIconForEntryType(entry.EntryType),
    SecondaryText = GetSecondaryText(entry)
};
```

---

## 📋 Definition of Done

- [ ] Všechny ViewModely pokryty testy
- [ ] Keyboard navigation funguje (Tab order, Enter, Esc)
- [ ] UI responzivní i při 1000+ entries
- [ ] Dark/Light theme funguje okamžitě
- [ ] Hotkeys fungují i když app není focus
- [ ] Accessibility: screen reader compatible
- [ ] High DPI support (per-monitor)
