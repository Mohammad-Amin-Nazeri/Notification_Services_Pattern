namespace NotificationServices.Sms.Abstractions.Models;

/// <summary>
/// Generic configuration shared by SMS providers.
/// Provider-specific values belong in <see cref="ProviderSettings"/> so the common contract
/// does not grow every time a new gateway has a unique option.
/// </summary>
public sealed class SmsProviderOptions
{
    public string ProviderType { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string From { get; set; } = string.Empty;

    /// <summary>
    /// Endpoint used for normal SMS messages by providers that expose an HTTP endpoint.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Endpoint used for OTP/pattern messages by providers that expose a dedicated endpoint.
    /// </summary>
    public string PatternBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Provider-specific settings. Keys and values are interpreted only by the selected provider.
    /// </summary>
    public Dictionary<string, string> ProviderSettings { get; set; }
        = new(StringComparer.OrdinalIgnoreCase);

    public string GetRequiredProviderSetting(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        if (!ProviderSettings.TryGetValue(key, out var value) ||
            string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"SMS provider setting '{key}' is required for provider '{ProviderType}'.");
        }

        return value;
    }
}
