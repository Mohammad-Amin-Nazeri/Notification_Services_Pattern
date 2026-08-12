namespace NotificationServices.Sms.Abstractions.Enums;

/// <summary>
/// Supported SMS gateway types.
/// Add a new value here whenever a new <c>ISmsProvider</c> implementation is introduced
/// (see <c>MelipayamakSmsProvider</c> in the NotificationServices.Sms project for an example).
/// </summary>
public enum SmsProviderType
{
    /// <summary>
    /// Melipayamak.com - reference/sample provider implementation shipped with this library.
    /// </summary>
    Melipayamak = 0,
}
