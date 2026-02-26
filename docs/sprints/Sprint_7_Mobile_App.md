# Sprint 7: Mobile App (.NET MAUI)

**Cíl:** iOS + Android app s biometrií, adaptivním UI, offline-first sync.

**Délka:** 8-10 dní  
**TDD přístup:** ✅ ANO pro ViewModely  
**Dependencies:** Sprint 6

---

## 📱 Platform Support

| Feature | iOS | Android |
|---------|-----|---------|
| Biometrie | Face ID / Touch ID | Fingerprint / Face Unlock |
| Secure Storage | Keychain | Keystore |
| Push Notifications | APNS | FCM |
| Widgets | iOS 14+ | Android 12+ |
| Auto-fill | Password Provider API | Auto-fill Framework |

---

## ✅ Tasky

### 1. MAUI Project Setup
**Testy:**
- [ ] `MauiAppTests.Initialization_Completes`

**Implementace:**
- [ ] `Passara.Mobile` projekt (MAUI App)
- [ ] `MauiProgram.cs` - DI container setup
- [ ] Platform-specific folders:
  - `Platforms/iOS/`
  - `Platforms/Android/`
- [ ] Shared resources (fonts, colors, icons)
- [ ] `App.xaml` - global styles

### 2. Mobile Navigation
**Implementace:**
- [ ] `Shell` navigation pattern:
  ```xml
  <Shell>
    <TabBar>
      <Tab Title="Trezor" Icon="vault.png">
        <ShellContent ContentTemplate="{DataTemplate local:VaultPage}" />
      </Tab>
      <Tab Title="Generátor" Icon="generator.png">
        <ShellContent ContentTemplate="{DataTemplate local:GeneratorPage}" />
      </Tab>
      <Tab Title="Nastavení" Icon="settings.png">
        <ShellContent ContentTemplate="{DataTemplate local:SettingsPage}" />
      </Tab>
    </TabBar>
  </Shell>
  ```

### 3. Mobile Authentication (Biometrie)
**Testy:**
- [ ] `BiometricServiceTests.Authenticate_Success_ReturnsTrue`
- [ ] `BiometricServiceTests.Authenticate_Cancelled_ReturnsFalse`

**Implementace:**
- [ ] `IBiometricService`:
  ```csharp
  public interface IBiometricService
  {
      Task<BiometricStatus> CheckAvailabilityAsync();
      Task<BiometricResult> AuthenticateAsync(string title, string message);
  }
  ```
- [ ] iOS: `LAContext` (LocalAuthentication framework)
- [ ] Android: `BiometricPrompt`

**Enums:**
```csharp
public enum BiometricStatus
{
    Available = 1,
    NotAvailable = 2,      // No hardware
    NotEnrolled = 3,       // No fingerprints/face registered
    PermissionDenied = 4
}

public enum BiometricResult
{
    Success = 1,
    Cancelled = 2,
    Failed = 3,
    TooManyAttempts = 4
}
```

### 4. Mobile-Specific UI Components
**Implementace:**
- [ ] `VaultListPage` - CollectionView s pull-to-refresh:
  ```xml
  <CollectionView ItemsSource="{Binding Entries}"
                  SelectionMode="Single"
                  SelectedItem="{Binding SelectedEntry}">
    <CollectionView.ItemTemplate>
      <DataTemplate>
        <SwipeView>
          <SwipeView.RightItems>
            <SwipeItem Text="Kopírovat"
                       IconImageSource="copy.png"
                       Command="{Binding CopyPasswordCommand}" />
          </SwipeView.RightItems>
          <Grid Padding="16,12">
            <Grid.ColumnDefinitions>
              <ColumnDefinition Width="48" /> <!-- Icon -->
              <ColumnDefinition Width="*" />  <!-- Content -->
              <ColumnDefinition Width="40" /> <!-- More -->
            </Grid.ColumnDefinitions>
            <!-- Content -->
          </Grid>
        </SwipeView>
      </DataTemplate>
    </CollectionView.ItemTemplate>
  </CollectionView>
  ```
- [ ] `EntryDetailPage` - scrollable detail
- [ ] `EditEntryPage` - formulář
- [ ] `PasswordGeneratorPage` - full-screen generator

### 5. Mobile Vault List UX
```
┌─────────────────────────────┐
│ 🔍 Vyhledat...       [⚙️]  │
├─────────────────────────────┤
│                             │
│ ┌─────────────────────────┐ │
│ │ [🔒] GitHub             │ │
│ │     developer@email.com │ │
│ │     [👤] [🔑] [📋]     │ │
│ └─────────────────────────┘ │
│                             │
│ ┌─────────────────────────┐ │
│ │ [📧] Gmail              │ │
│ │     personal@gmail.com  │ │
│ │     [👤] [🔑] [🔢]     │ │
│ └─────────────────────────┘ │
│                             │
│ ◀  All (12)  ▶              │
│                             │
├─────────────────────────────┤
│ [🏠] [🔧] [⚙️]              │
└─────────────────────────────┘
```

**Interakce:**
- Tap = otevřít detail
- Swipe right = kopírovat uživatel
- Swipe left = kopírovat heslo
- Long press = context menu
- Pull down = sync

### 6. Entry Detail (Mobile)
```
┌─────────────────────────────┐
│ [←] GitHub           [✏️]  │
├─────────────────────────────┤
│                             │
│     [🔒]                    │
│      GitHub                 │
│   github.com/login          │
│                             │
├─────────────────────────────┤
│ 👤 Uživatel                 │
│ ┌─────────────────────────┐ │
│ │ developer@email.com     │ │
│ │              [📋]       │ │
│ └─────────────────────────┘ │
│                             │
│ 🔑 Heslo                    │
│ ┌─────────────────────────┐ │
│ │ ••••••••••••••         │ │
│ │   [👁️] [📋] [🔄]       │ │
│ └─────────────────────────┘ │
│ [████████] 85 bitů          │
│                             │
│ 🔢 TOTP                     │
│ ┌─────────────────────────┐ │
│ │ 123 456    [⏱️ 18s]     │ │
│ │              [📋]       │ │
│ └─────────────────────────┘ │
│                             │
│ 📝 Poznámky                 │
│ ┌─────────────────────────┐ │
│ │ 2FA enabled             │ │
│ │ Recovery codes in safe  │ │
│ └─────────────────────────┘ │
│                             │
│ 📎 Přílohy (2)              │
│                             │
│ [🗑️ Smazat]  [🌐 Otevřít]  │
│                             │
└─────────────────────────────┘
```

### 7. Quick Actions / App Shortcuts
**iOS:**
- 3D Touch / Haptic Touch menu:
  - Search Vault
  - Generate Password
  - Copy Last Used

**Android:**
- App shortcuts:
  ```xml
  <shortcut
    android:shortcutId="search"
    android:enabled="true"
    android:icon="@drawable/ic_search"
    android:shortcutShortLabel="@string/search">
    <intent ... />
  </shortcut>
  ```

### 8. Mobile Sync
**Testy:**
- [ ] `MobileSyncTests.BackgroundSync_UpdatesBadge`

**Implementace:**
- [ ] Background sync (iOS: BGTaskScheduler, Android: WorkManager)
- [ ] Push notification při konfliktu:
  ```
  "Konflikt synchronizace: GitHub bylo změněno na jiném zařízení"
  ```
- [ ] Badge na app icon s počtem konfliktů
- [ ] Offline indicator (toast když není síť)

### 9. Platform-Specific Secure Storage
**Implementace:**
- [ ] `ISecureKeyStorage`:
  ```csharp
  public interface ISecureKeyStorage
  {
      Task StoreAsync(string key, byte[] data);
      Task<byte[]> RetrieveAsync(string key);
      Task DeleteAsync(string key);
  }
  ```
- [ ] iOS: `SecKeyChain`
- [ ] Android: `AndroidKeystore` + encrypted SharedPreferences
- [ ] Uložení:
  - Master key (encrypted, biometrie protected)
  - Sync credentials (tokeny pro cloud)
  - App settings (ne secrets)

### 10. Mobile Password Generator
```
┌─────────────────────────────┐
│ Generátor hesla      [✓]   │
├─────────────────────────────┤
│                             │
│ ┌─────────────────────────┐ │
│ │                         │ │
│ │   Tr0ub4dor&3!          │ │
│ │                         │ │
│ │   [🔄]  [📋]           │ │
│ └─────────────────────────┘ │
│                             │
│ Délka                       │
│ [━━━●━━━━] 16 znaků        │
│                             │
│ ☑️ ABC  ☑️ abc  ☑️ 123      │
│ ☑️ !@#  ☐ Mezery            │
│                             │
│ Síla: [████████░░] 78 bitů │
│                             │
│ [💾 Uložit do trezoru]     │
└─────────────────────────────┘
```

---

## 🎨 Mobile Design Guidelines

### Touch Targets
- Minimum 44×44 pt (iOS)
- Minimum 48×48 dp (Android)
- Spacing mezi elementy: 8dp/pt

### Typography
```xml
<!-- Naming convention -->
<Style x:Key="Headline" TargetType="Label">
    <Setter Property="FontSize" Value="24" />
    <Setter Property="FontAttributes" Value="Bold" />
</Style>
<Style x:Key="Title" TargetType="Label">
    <Setter Property="FontSize" Value="20" />
</Style>
<Style x:Key="Body" TargetType="Label">
    <Setter Property="FontSize" Value="16" />
</Style>
<Style x:Key="Caption" TargetType="Label">
    <Setter Property="FontSize" Value="12" />
    <Setter Property="TextColor" Value="{StaticResource SecondaryTextColor}" />
</Style>
```

### Platform Conventions
- **iOS**: 
  - Navigation bar s back button
  - Tab bar na spodku
  - Settings gear v top right
  - iOS-style switches
- **Android**:
  - Material Design 3
  - Floating Action Button (FAB) pro "Add"
  - Top app bar s hamburger menu
  - Bottom navigation

---

## 📋 Definition of Done

- [ ] App funguje na iOS 14+ a Android 10+
- [ ] Biometrie funguje na obou platformách
- [ ] Swipe actions na listu fungují plynule
- [ ] Pull-to-refresh sync funguje
- [ ] Offline mode - všechny operace se queue
- [ ] Background sync probíhá každých 15 min
- [ ] Battery optimization: sync spotřebuje < 1% za den
- [ ] App Store / Play Store ready metadata
