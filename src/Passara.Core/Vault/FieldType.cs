namespace Passara.Core.Vault;

/// <summary>
/// Defines the types of fields that can appear in a vault entry.
/// </summary>
public enum FieldType
{
    Text = 1,
    Password = 2,
    Email = 3,
    Url = 4,
    Phone = 5,
    Date = 6,
    Number = 7,
    Boolean = 8,
    Multiline = 9,
    Totp = 10,
    Tags = 11,
    Color = 12
}
