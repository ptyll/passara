namespace Passara.Core.Vault;

/// <summary>
/// Defines the cloud storage providers supported for synchronization.
/// </summary>
public enum SyncProviderType
{
    LocalFolder = 1,
    OneDrive = 2,
    GoogleDrive = 3,
    Dropbox = 4,
    ICloud = 5,
    WebDav = 6,
    S3Compatible = 7,
    Nextcloud = 8
}
