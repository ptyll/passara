using System.Globalization;
using System.Resources;
using Passara.Core.Resources;

namespace Passara.Core.Localization;

/// <summary>
/// Implementation of <see cref="ILocalizationService"/> using .NET ResourceManager.
/// </summary>
public class ResourceManagerLocalizationService : ILocalizationService
{
    private readonly ResourceManager _resourceManager;
    private CultureInfo _currentCulture;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceManagerLocalizationService"/> class.
    /// </summary>
    public ResourceManagerLocalizationService()
    {
        _resourceManager = Resources.Resources.ResourceManager;
        _currentCulture = CultureInfo.CurrentUICulture;
    }

    /// <inheritdoc />
    public string this[string key]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            var value = _resourceManager.GetString(key, _currentCulture);
            return value ?? key;
        }
    }

    /// <inheritdoc />
    public string GetString(string key, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return string.Empty;
        }

        var value = _resourceManager.GetString(key, _currentCulture);
        if (value is null)
        {
            return key;
        }

        return args.Length > 0
            ? string.Format(_currentCulture, value, args)
            : value;
    }

    /// <inheritdoc />
    public string CurrentCulture => _currentCulture.Name;

    /// <inheritdoc />
    public void SetCulture(string cultureCode)
    {
        if (string.IsNullOrWhiteSpace(cultureCode))
        {
            throw new ArgumentException("Culture code cannot be null or empty.", nameof(cultureCode));
        }

        _currentCulture = CultureInfo.GetCultureInfo(cultureCode);
        CultureInfo.CurrentCulture = _currentCulture;
        CultureInfo.CurrentUICulture = _currentCulture;
    }
}
