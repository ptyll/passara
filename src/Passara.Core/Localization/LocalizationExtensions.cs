namespace Passara.Core.Localization;

/// <summary>
/// Provides extension methods for localization.
/// </summary>
public static class LocalizationExtensions
{
    /// <summary>
    /// Gets the localized string for the specified error code.
    /// </summary>
    /// <param name="localizationService">The localization service.</param>
    /// <param name="errorCode">The error code to localize.</param>
    /// <returns>The localized error message.</returns>
    /// <exception cref="ArgumentNullException">Thrown when localizationService is null.</exception>
    public static string L(this ILocalizationService localizationService, Common.ErrorCode errorCode)
    {
        if (localizationService is null)
        {
            throw new ArgumentNullException(nameof(localizationService));
        }

        var key = $"Error_{errorCode}";
        return localizationService[key];
    }

    /// <summary>
    /// Gets the localized string for the specified resource key.
    /// </summary>
    /// <param name="localizationService">The localization service.</param>
    /// <param name="key">The resource key.</param>
    /// <returns>The localized string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when localizationService is null.</exception>
    public static string L(this ILocalizationService localizationService, string key)
    {
        if (localizationService is null)
        {
            throw new ArgumentNullException(nameof(localizationService));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
        }

        return localizationService[key];
    }

    /// <summary>
    /// Gets the formatted localized string for the specified resource key.
    /// </summary>
    /// <param name="localizationService">The localization service.</param>
    /// <param name="key">The resource key.</param>
    /// <param name="args">The format arguments.</param>
    /// <returns>The formatted localized string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when localizationService is null.</exception>
    public static string L(this ILocalizationService localizationService, string key, params object[] args)
    {
        if (localizationService is null)
        {
            throw new ArgumentNullException(nameof(localizationService));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
        }

        return localizationService.GetString(key, args);
    }
}
