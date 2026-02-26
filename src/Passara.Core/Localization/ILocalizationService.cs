namespace Passara.Core.Localization;

/// <summary>
/// Defines the contract for localization services.
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Gets the localized string for the specified key.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <returns>The localized string, or the key itself if not found.</returns>
    string this[string key] { get; }

    /// <summary>
    /// Gets the localized string for the specified key and formats it with the provided arguments.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <param name="args">The format arguments.</param>
    /// <returns>The formatted localized string.</returns>
    string GetString(string key, params object[] args);

    /// <summary>
    /// Gets the current culture code (e.g., "en", "cs", "de").
    /// </summary>
    string CurrentCulture { get; }

    /// <summary>
    /// Changes the current culture.
    /// </summary>
    /// <param name="cultureCode">The culture code to switch to.</param>
    void SetCulture(string cultureCode);
}
