namespace Passara.Core.Vault;

/// <summary>
/// Defines the types of entries that can be stored in a vault.
/// </summary>
public enum EntryType
{
    Password = 1,
    SecureNote = 2,
    CreditCard = 3,
    Identity = 4,
    BankAccount = 5,
    Passport = 6,
    DriversLicense = 7,
    SoftwareLicense = 8,
    SshKey = 9,
    Database = 10,
    ApiCredential = 11,
    WifiPassword = 12
}
