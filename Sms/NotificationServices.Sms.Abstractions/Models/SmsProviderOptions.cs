namespace NotificationServices.Sms.Abstractions.Models;

/// <summary>
/// Configuration required by an SMS provider.
/// </summary>
public sealed class SmsProviderOptions
{
    public string ProviderType { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string From { get; set; } = string.Empty;

    /// <summary>
    /// Endpoint used for normal SMS messages.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Endpoint used for pattern/OTP messages.
    /// </summary>
    public string PatternBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Provider-specific template/body identifier used for OTP messages.
    /// </summary>
    public string BodyId { get; set; } = string.Empty;
}