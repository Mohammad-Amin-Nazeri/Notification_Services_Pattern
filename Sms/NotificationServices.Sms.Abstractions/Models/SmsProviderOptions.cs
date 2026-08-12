namespace NotificationServices.Sms.Abstractions.Models;

/// <summary>
/// Credentials/settings required to talk to the configured SMS gateway.
/// By default this is bound from the "SmsProvider" section of your configuration
/// (see <c>AppSettingsSmsProviderOptionsProvider</c>), but any <c>ISmsProviderOptionsProvider</c>
/// implementation (e.g. one backed by a database) can populate it instead.
/// </summary>
public class SmsProviderOptions
{
    /// <summary>Name of a <see cref="Enums.SmsProviderType"/> value, e.g. "Melipayamak".</summary>
    public string ProviderType { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string From { get; set; } = null!;
    public string BodyId { get; set; } = null!;
}
